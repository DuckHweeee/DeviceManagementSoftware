using Microsoft.EntityFrameworkCore;
using DeviceManagementSoftware.Data;
using DeviceManagementSoftware.Models;

namespace DeviceManagementSoftware.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Where(u => u.IsActive)
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetUserWithDevicesAsync(int id)
        {
            return await _context.Users
                .Include(u => u.UserDevices!)
                    .ThenInclude(ud => ud.Device)
                        .ThenInclude(d => d!.Category)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<User>> SearchUsersAsync(string? searchTerm, string? department, bool activeOnly = true)
        {
            var query = _context.Users.AsQueryable();

            if (activeOnly)
            {
                query = query.Where(u => u.IsActive);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(u => u.FullName.Contains(searchTerm) || u.Email.Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(department))
            {
                query = query.Where(u => u.Department != null && u.Department.Contains(department));
            }

            return await query.OrderBy(u => u.FullName).ToListAsync();
        }

        public async Task<(IEnumerable<User> users, int totalCount)> GetPagedUsersAsync(
            int pageNumber, int pageSize, string? searchTerm = null, 
            string? department = null, bool activeOnly = true)
        {
            var query = _context.Users.AsQueryable();

            if (activeOnly)
            {
                query = query.Where(u => u.IsActive);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(u => u.FullName.Contains(searchTerm) || u.Email.Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(department))
            {
                query = query.Where(u => u.Department != null && u.Department.Contains(department));
            }

            var totalCount = await query.CountAsync();
            var users = await query
                .OrderBy(u => u.FullName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, totalCount);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            user.DateCreated = DateTime.Now;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.UserDevices)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return false;

            // Check if user has active device assignments
            if (user.UserDevices?.Any(ud => ud.IsActive) == true)
                throw new InvalidOperationException("Cannot delete user who has active device assignments.");

            // Soft delete by marking as inactive
            user.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UserExistsAsync(int id)
        {
            return await _context.Users.AnyAsync(u => u.Id == id);
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        {
            var query = _context.Users.Where(u => u.Email.ToLower() == email.ToLower());
            
            if (excludeId.HasValue)
                query = query.Where(u => u.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<UserDevice> AssignDeviceToUserAsync(int userId, int deviceId, string? notes = null)
        {
            // Check if device is already assigned to someone else
            var existingAssignment = await _context.UserDevices
                .Where(ud => ud.DeviceId == deviceId && ud.IsActive)
                .FirstOrDefaultAsync();

            if (existingAssignment != null)
                throw new InvalidOperationException("Device is already assigned to another user.");

            var userDevice = new UserDevice
            {
                UserId = userId,
                DeviceId = deviceId,
                AssignedDate = DateTime.Now,
                Notes = notes,
                IsActive = true
            };

            _context.UserDevices.Add(userDevice);

            // Update device status to InUse
            var device = await _context.Devices.FindAsync(deviceId);
            if (device != null)
            {
                device.Status = DeviceStatus.InUse;
            }

            await _context.SaveChangesAsync();
            return userDevice;
        }

        public async Task<bool> ReturnDeviceAsync(int userDeviceId)
        {
            var userDevice = await _context.UserDevices
                .Include(ud => ud.Device)
                .FirstOrDefaultAsync(ud => ud.Id == userDeviceId);

            if (userDevice == null || !userDevice.IsActive)
                return false;

            userDevice.ReturnDate = DateTime.Now;
            userDevice.IsActive = false;

            // Update device status to Available
            if (userDevice.Device != null)
            {
                userDevice.Device.Status = DeviceStatus.Available;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<UserDevice>> GetUserDeviceHistoryAsync(int userId)
        {
            return await _context.UserDevices
                .Include(ud => ud.Device)
                    .ThenInclude(d => d!.Category)
                .Where(ud => ud.UserId == userId)
                .OrderByDescending(ud => ud.AssignedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserDevice>> GetActiveUserDevicesAsync(int userId)
        {
            return await _context.UserDevices
                .Include(ud => ud.Device)
                    .ThenInclude(d => d!.Category)
                .Where(ud => ud.UserId == userId && ud.IsActive)
                .OrderBy(ud => ud.AssignedDate)
                .ToListAsync();
        }

        public async Task<UserDevice?> GetActiveAssignmentForDeviceAsync(int deviceId)
        {
            return await _context.UserDevices
                .Include(ud => ud.User)
                .Include(ud => ud.Device)
                .FirstOrDefaultAsync(ud => ud.DeviceId == deviceId && ud.IsActive);
        }
    }
}
