using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DeviceManagementSoftware.Models;
using DeviceManagementSoftware.Services;

namespace DeviceManagementSoftware.Controllers
{
    public class DeviceCategoryController : Controller
    {
        private readonly IDeviceCategoryService _categoryService;

        public DeviceCategoryController(IDeviceCategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: DeviceCategory
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories);
        }

        // GET: DeviceCategory/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: DeviceCategory/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DeviceCategory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DeviceCategory category)
        {
            if (ModelState.IsValid)
            {
                // Check if category name already exists
                if (await _categoryService.CategoryNameExistsAsync(category.Name))
                {
                    ModelState.AddModelError("Name", "A category with this name already exists.");
                    return View(category);
                }

                try
                {
                    await _categoryService.CreateCategoryAsync(category);
                    TempData["SuccessMessage"] = "Device category created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                }
            }

            return View(category);
        }

        // GET: DeviceCategory/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: DeviceCategory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DeviceCategory category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Check if category name already exists (excluding current category)
                if (await _categoryService.CategoryNameExistsAsync(category.Name, category.Id))
                {
                    ModelState.AddModelError("Name", "A category with this name already exists.");
                    return View(category);
                }

                try
                {
                    await _categoryService.UpdateCategoryAsync(category);
                    TempData["SuccessMessage"] = "Device category updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                }
            }

            return View(category);
        }

        // GET: DeviceCategory/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: DeviceCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);
                if (result)
                {
                    TempData["SuccessMessage"] = "Device category deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Category not found.";
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
    }
}
