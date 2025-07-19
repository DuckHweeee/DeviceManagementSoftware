using DeviceManagementSoftware.Models;

namespace DeviceManagementSoftware.Services
{
    public interface IDeviceService
    {
        Task<IEnumerable<Device>> GetAllDevicesAsync();
        Task<Device?> GetDeviceByIdAsync(int id);
        Task<Device?> GetDeviceWithDetailsAsync(int id);
        Task<IEnumerable<Device>> GetDevicesByCategoryAsync(int categoryId);
        Task<IEnumerable<Device>> SearchDevicesAsync(string? searchTerm, int? categoryId, DeviceStatus? status, string? location);
        Task<Device> CreateDeviceAsync(Device device);
        Task<Device> UpdateDeviceAsync(Device device);
        Task<bool> DeleteDeviceAsync(int id);
        Task<bool> DeviceExistsAsync(int id);
        Task<bool> DeviceCodeExistsAsync(string code, int? excludeId = null);
        Task<(IEnumerable<Device> devices, int totalCount)> GetPagedDevicesAsync(
            int pageNumber, int pageSize, string? searchTerm = null, 
            int? categoryId = null, DeviceStatus? status = null, string? location = null);
        Task<string?> ValidateDeviceStatusBusinessRuleAsync(int deviceId, DeviceStatus newStatus);
    }
}
