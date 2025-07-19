using DeviceManagementSoftware.Models;

namespace DeviceManagementSoftware.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserWithDevicesAsync(int id);
        Task<IEnumerable<User>> SearchUsersAsync(string? searchTerm, string? department, bool activeOnly = true);
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> UserExistsAsync(int id);
        Task<bool> EmailExistsAsync(string email, int? excludeId = null);
        Task<(IEnumerable<User> users, int totalCount)> GetPagedUsersAsync(
            int pageNumber, int pageSize, string? searchTerm = null, 
            string? department = null, bool activeOnly = true);
        Task<UserDevice> AssignDeviceToUserAsync(int userId, int deviceId, string? notes = null);
        Task<bool> ReturnDeviceAsync(int userDeviceId);
        Task<IEnumerable<UserDevice>> GetUserDeviceHistoryAsync(int userId);
        Task<IEnumerable<UserDevice>> GetActiveUserDevicesAsync(int userId);
        Task<UserDevice?> GetActiveAssignmentForDeviceAsync(int deviceId);
    }
}
