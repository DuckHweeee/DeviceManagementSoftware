using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DeviceManagementSoftware.Models;
using DeviceManagementSoftware.Models.ViewModels;
using DeviceManagementSoftware.Services;

namespace DeviceManagementSoftware.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IDeviceService _deviceService;

        public UserController(IUserService userService, IDeviceService deviceService)
        {
            _userService = userService;
            _deviceService = deviceService;
        }

        // GET: User
        public async Task<IActionResult> Index(string? searchTerm, string? department, bool activeOnly = true, int pageNumber = 1, int pageSize = 10)
        {
            var (users, totalCount) = await _userService.GetPagedUsersAsync(pageNumber, pageSize, searchTerm, department, activeOnly);

            var viewModel = new UserSearchFilterViewModel
            {
                SearchTerm = searchTerm,
                Department = department,
                ActiveOnly = activeOnly,
                Users = users,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return View(viewModel);
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userService.GetUserWithDevicesAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new UserDetailsViewModel
            {
                User = user,
                AssignedDevices = await _userService.GetActiveUserDevicesAsync(id),
                DeviceHistory = await _userService.GetUserDeviceHistoryAsync(id)
            };

            return View(viewModel);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists
                if (await _userService.EmailExistsAsync(user.Email))
                {
                    ModelState.AddModelError("Email", "A user with this email already exists.");
                    return View(user);
                }

                try
                {
                    await _userService.CreateUserAsync(user);
                    TempData["SuccessMessage"] = "User created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                }
            }

            return View(user);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Check if email already exists (excluding current user)
                if (await _userService.EmailExistsAsync(user.Email, user.Id))
                {
                    ModelState.AddModelError("Email", "A user with this email already exists.");
                    return View(user);
                }

                try
                {
                    await _userService.UpdateUserAsync(user);
                    TempData["SuccessMessage"] = "User updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                }
            }

            return View(user);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);
                if (result)
                {
                    TempData["SuccessMessage"] = "User deactivated successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "User not found.";
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

        // GET: User/AssignDevice
        public async Task<IActionResult> AssignDevice()
        {
            var users = await _userService.GetAllUsersAsync();
            var devices = await _deviceService.GetAllDevicesAsync();
            var availableDevices = devices.Where(d => d.Status == DeviceStatus.Available).ToList();

            // Debug information
            ViewBag.TotalDevices = devices.Count();
            ViewBag.AvailableDevicesCount = availableDevices.Count;

            var viewModel = new AssignDeviceViewModel
            {
                Users = new SelectList(users, "Id", "FullName"),
                AvailableDevices = new SelectList(availableDevices, "Id", "Name")
            };

            return View(viewModel);
        }

        // POST: User/AssignDevice
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignDevice(AssignDeviceViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _userService.AssignDeviceToUserAsync(viewModel.UserId, viewModel.DeviceId, viewModel.Notes);
                    TempData["SuccessMessage"] = "Device assigned successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                }
            }

            // Reload dropdowns
            var users = await _userService.GetAllUsersAsync();
            var devices = await _deviceService.GetAllDevicesAsync();
            var availableDevices = devices.Where(d => d.Status == DeviceStatus.Available).ToList();

            // Debug information
            ViewBag.TotalDevices = devices.Count();
            ViewBag.AvailableDevicesCount = availableDevices.Count;

            viewModel.Users = new SelectList(users, "Id", "FullName", viewModel.UserId);
            viewModel.AvailableDevices = new SelectList(availableDevices, "Id", "Name", viewModel.DeviceId);

            return View(viewModel);
        }

        // POST: User/ReturnDevice/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnDevice(int userDeviceId)
        {
            try
            {
                var result = await _userService.ReturnDeviceAsync(userDeviceId);
                if (result)
                {
                    TempData["SuccessMessage"] = "Device returned successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Assignment not found or device already returned.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
