using DeviceManagementSoftware.Models;
using DeviceManagementSoftware.Models.ViewModels;

namespace DeviceManagementSoftware.Services
{
    public interface IDeviceCategoryService
    {
        Task<IEnumerable<DeviceCategory>> GetAllCategoriesAsync();
        Task<PaginatedResult<DeviceCategory>> GetPaginatedCategoriesAsync(int pageNumber = 1, int pageSize = 10);
        Task<DeviceCategory?> GetCategoryByIdAsync(int id);
        Task<DeviceCategory> CreateCategoryAsync(DeviceCategory category);
        Task<DeviceCategory> UpdateCategoryAsync(DeviceCategory category);
        Task<bool> DeleteCategoryAsync(int id);
        Task<bool> CategoryExistsAsync(int id);
        Task<bool> CategoryNameExistsAsync(string name, int? excludeId = null);
    }
}
