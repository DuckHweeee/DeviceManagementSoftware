using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DeviceManagementSoftware.Models;
using DeviceManagementSoftware.Models.ViewModels;
using DeviceManagementSoftware.Services;

namespace DeviceManagementSoftware.Controllers
{
    public class DeviceController : Controller
    {
        private readonly IDeviceService _deviceService;
        private readonly IDeviceCategoryService _categoryService;
        private readonly IUserService _userService;

        public DeviceController(IDeviceService deviceService, IDeviceCategoryService categoryService, IUserService userService)
        {
            _deviceService = deviceService;
            _categoryService = categoryService;
            _userService = userService;
        }

        // GET: Device
        public async Task<IActionResult> Index(string? searchTerm, int? categoryId, DeviceStatus? status, string? location, int pageNumber = 1, int pageSize = 10)
        {
            var (devices, totalCount) = await _deviceService.GetPagedDevicesAsync(pageNumber, pageSize, searchTerm, categoryId, status, location);
            var categories = await _categoryService.GetAllCategoriesAsync();

            var viewModel = new DeviceSearchFilterViewModel
            {
                SearchTerm = searchTerm,
                CategoryId = categoryId,
                Status = status,
                Location = location,
                Categories = new SelectList(categories, "Id", "Name", categoryId),
                Statuses = new SelectList(Enum.GetValues<DeviceStatus>().Select(s => new { Value = s, Text = s.ToString() }), "Value", "Text", status),
                Devices = devices,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return View(viewModel);
        }

        // GET: Device/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var device = await _deviceService.GetDeviceWithDetailsAsync(id);
            if (device == null)
            {
                return NotFound();
            }

            var viewModel = new DeviceDetailsViewModel
            {
                Device = device,
                AssignedUser = device.UserDevices?.FirstOrDefault(ud => ud.IsActive)?.User,
                AssignmentHistory = device.UserDevices?.OrderByDescending(ud => ud.AssignedDate)
            };

            return View(viewModel);
        }

        // GET: Device/Create
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            var viewModel = new DeviceCreateEditViewModel
            {
                Categories = new SelectList(categories, "Id", "Name"),
                Statuses = new SelectList(Enum.GetValues<DeviceStatus>().Select(s => new { Value = s, Text = s.ToString() }), "Value", "Text")
            };

            return View(viewModel);
        }

        // POST: Device/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DeviceCreateEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Check if device code already exists
                if (await _deviceService.DeviceCodeExistsAsync(viewModel.Device.Code))
                {
                    ModelState.AddModelError("Device.Code", "A device with this code already exists.");
                }
                else
                {
                    try
                    {
                        await _deviceService.CreateDeviceAsync(viewModel.Device);
                        TempData["SuccessMessage"] = "Device created successfully.";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                    }
                }
            }

            // Reload dropdowns
            var categories = await _categoryService.GetAllCategoriesAsync();
            viewModel.Categories = new SelectList(categories, "Id", "Name", viewModel.Device.CategoryId);
            viewModel.Statuses = new SelectList(Enum.GetValues<DeviceStatus>().Select(s => new { Value = s, Text = s.ToString() }), "Value", "Text", viewModel.Device.Status);

            return View(viewModel);
        }

        // GET: Device/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var device = await _deviceService.GetDeviceWithDetailsAsync(id);
            if (device == null)
            {
                return NotFound();
            }

            // Get current assignment
            var currentAssignment = device.UserDevices?.FirstOrDefault(ud => ud.IsActive);

            var categories = await _categoryService.GetAllCategoriesAsync();
            var users = await _userService.GetAllUsersAsync();

            var viewModel = new DeviceCreateEditViewModel
            {
                Device = device,
                Categories = new SelectList(categories, "Id", "Name", device.CategoryId),
                Statuses = new SelectList(Enum.GetValues<DeviceStatus>().Select(s => new { Value = s, Text = s.ToString() }), "Value", "Text", device.Status),
                AssignedUser = currentAssignment?.User,
                CurrentAssignment = currentAssignment,
                AvailableUsers = new SelectList(users, "Id", "FullName"),
                SelectedUserId = currentAssignment?.UserId
            };

            return View(viewModel);
        }

        // POST: Device/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DeviceCreateEditViewModel viewModel)
        {
            if (id != viewModel.Device.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Check if device code already exists (excluding current device)
                if (await _deviceService.DeviceCodeExistsAsync(viewModel.Device.Code, viewModel.Device.Id))
                {
                    ModelState.AddModelError("Device.Code", "A device with this code already exists.");
                }
                else
                {
                    // Validate business rule for device status
                    var businessRuleError = await _deviceService.ValidateDeviceStatusBusinessRuleAsync(viewModel.Device.Id, viewModel.Device.Status);
                    if (!string.IsNullOrEmpty(businessRuleError))
                    {
                        ModelState.AddModelError("Device.Status", businessRuleError);
                    }
                    else
                    {
                        try
                        {
                            // Get current assignment BEFORE updating the device to avoid tracking conflicts
                            var currentAssignmentBefore = await _userService.GetActiveAssignmentForDeviceAsync(viewModel.Device.Id);

                            await _deviceService.UpdateDeviceAsync(viewModel.Device);

                            // Handle assignment changes based on pre-update state
                            // If user wants to assign to someone and no current assignment
                            if (viewModel.SelectedUserId.HasValue && currentAssignmentBefore == null)
                            {
                                await _userService.AssignDeviceToUserAsync(viewModel.SelectedUserId.Value, viewModel.Device.Id, viewModel.AssignmentNotes);
                                TempData["SuccessMessage"] = "Device updated and assigned to user successfully.";
                            }
                            // If user wants to change assignment
                            else if (viewModel.SelectedUserId.HasValue && currentAssignmentBefore != null && currentAssignmentBefore.UserId != viewModel.SelectedUserId.Value)
                            {
                                // Return current device first
                                await _userService.ReturnDeviceAsync(currentAssignmentBefore.Id);
                                // Then assign to new user
                                await _userService.AssignDeviceToUserAsync(viewModel.SelectedUserId.Value, viewModel.Device.Id, viewModel.AssignmentNotes);
                                TempData["SuccessMessage"] = "Device updated and reassigned successfully.";
                            }
                            // If user wants to unassign (no user selected but there's current assignment)
                            else if (!viewModel.SelectedUserId.HasValue && currentAssignmentBefore != null)
                            {
                                await _userService.ReturnDeviceAsync(currentAssignmentBefore.Id);
                                TempData["SuccessMessage"] = "Device updated and unassigned from user.";
                            }
                            else
                            {
                                TempData["SuccessMessage"] = "Device updated successfully.";
                            }

                            return RedirectToAction(nameof(Index));
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                        }
                    }
                }
            }

            // Reload dropdowns and assignment info
            var categories = await _categoryService.GetAllCategoriesAsync();
            var users = await _userService.GetAllUsersAsync();
            var currentAssignment = await _userService.GetActiveAssignmentForDeviceAsync(viewModel.Device.Id);

            viewModel.Categories = new SelectList(categories, "Id", "Name", viewModel.Device.CategoryId);
            viewModel.Statuses = new SelectList(Enum.GetValues<DeviceStatus>().Select(s => new { Value = s, Text = s.ToString() }), "Value", "Text", viewModel.Device.Status);
            viewModel.AvailableUsers = new SelectList(users, "Id", "FullName");
            viewModel.AssignedUser = currentAssignment?.User;
            viewModel.CurrentAssignment = currentAssignment;

            return View(viewModel);
        }

        // GET: Device/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var device = await _deviceService.GetDeviceByIdAsync(id);
            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }

        // POST: Device/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _deviceService.DeleteDeviceAsync(id);
                if (result)
                {
                    TempData["SuccessMessage"] = "Device deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Device not found.";
                }
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Device/ByCategory/5
        public async Task<IActionResult> ByCategory(int id, int page = 1, int pageSize = 10)
        {
            // Validate page and pageSize parameters
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var (devices, totalCount) = await _deviceService.GetPagedDevicesAsync(page, pageSize, null, id, null, null);
            ViewBag.CategoryName = category.Name;
            ViewBag.CategoryId = id;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < ViewBag.TotalPages;

            return View(devices);
        }
    }
}
