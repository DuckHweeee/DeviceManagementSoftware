using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DeviceManagementSoftware.Models;
using DeviceManagementSoftware.Services;

namespace DeviceManagementSoftware.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IDeviceService _deviceService;
    private readonly IUserService _userService;
    private readonly IDeviceCategoryService _categoryService;

    public HomeController(ILogger<HomeController> logger, IDeviceService deviceService,
        IUserService userService, IDeviceCategoryService categoryService)
    {
        _logger = logger;
        _deviceService = deviceService;
        _userService = userService;
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 6)
    {
        // Validate page and pageSize parameters
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 50) pageSize = 6; // Reasonable limits
        
        var devices = await _deviceService.GetAllDevicesAsync();
        var users = await _userService.GetAllUsersAsync();
        var paginatedCategories = await _categoryService.GetPaginatedCategoriesAsync(page, pageSize);

        ViewBag.TotalDevices = devices.Count();
        ViewBag.InUseDevices = devices.Count(d => d.Status == DeviceStatus.InUse);
        ViewBag.AvailableDevices = devices.Count(d => d.Status == DeviceStatus.Available);
        ViewBag.BrokenDevices = devices.Count(d => d.Status == DeviceStatus.Broken);
        ViewBag.TotalUsers = users.Count();
        ViewBag.TotalCategories = paginatedCategories.TotalCount;
        ViewBag.Categories = paginatedCategories.Items;
        ViewBag.CategoryPagination = paginatedCategories;

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
