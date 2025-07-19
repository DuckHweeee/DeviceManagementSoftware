using DeviceManagementSoftware.Models;

namespace DeviceManagementSoftware.Services
{
    public interface IDeviceCategoryService
    {
        Task<IEnumerable<DeviceCategory>> GetAllCategoriesAsync();
        Task<DeviceCategory?> GetCategoryByIdAsync(int id);
        Task<DeviceCategory> CreateCategoryAsync(DeviceCategory category);
        Task<DeviceCategory> UpdateCategoryAsync(DeviceCategory category);
        Task<bool> DeleteCategoryAsync(int id);
        Task<bool> CategoryExistsAsync(int id);
        Task<bool> CategoryNameExistsAsync(string name, int? excludeId = null);
    }
}
