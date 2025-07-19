using Microsoft.EntityFrameworkCore;
using DeviceManagementSoftware.Data;
using DeviceManagementSoftware.Models;

namespace DeviceManagementSoftware.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly ApplicationDbContext _context;

        public DeviceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Device>> GetAllDevicesAsync()
        {
            return await _context.Devices
                .Include(d => d.Category)
                .Include(d => d.UserDevices!.Where(ud => ud.IsActive))
                    .ThenInclude(ud => ud.User)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<Device?> GetDeviceByIdAsync(int id)
        {
            return await _context.Devices
                .Include(d => d.Category)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Device?> GetDeviceWithDetailsAsync(int id)
        {
            return await _context.Devices
                .Include(d => d.Category)
                .Include(d => d.UserDevices!)
                    .ThenInclude(ud => ud.User)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<Device>> GetDevicesByCategoryAsync(int categoryId)
        {
            return await _context.Devices
                .Include(d => d.Category)
                .Where(d => d.CategoryId == categoryId)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Device>> SearchDevicesAsync(string? searchTerm, int? categoryId, DeviceStatus? status, string? location)
        {
            var query = _context.Devices
                .Include(d => d.Category)
                .Include(d => d.UserDevices!.Where(ud => ud.IsActive))
                    .ThenInclude(ud => ud.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(d => d.Name.Contains(searchTerm) || d.Code.Contains(searchTerm));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(d => d.CategoryId == categoryId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(d => d.Status == status.Value);
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                query = query.Where(d => d.Location != null && d.Location.Contains(location));
            }

            return await query.OrderBy(d => d.Name).ToListAsync();
        }

        public async Task<(IEnumerable<Device> devices, int totalCount)> GetPagedDevicesAsync(
            int pageNumber, int pageSize, string? searchTerm = null, 
            int? categoryId = null, DeviceStatus? status = null, string? location = null)
        {
            var query = _context.Devices
                .Include(d => d.Category)
                .Include(d => d.UserDevices!.Where(ud => ud.IsActive))
                    .ThenInclude(ud => ud.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(d => d.Name.Contains(searchTerm) || d.Code.Contains(searchTerm));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(d => d.CategoryId == categoryId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(d => d.Status == status.Value);
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                query = query.Where(d => d.Location != null && d.Location.Contains(location));
            }

            var totalCount = await query.CountAsync();
            var devices = await query
                .OrderBy(d => d.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (devices, totalCount);
        }

        public async Task<Device> CreateDeviceAsync(Device device)
        {
            device.DateOfEntry = DateTime.Now;
            _context.Devices.Add(device);
            await _context.SaveChangesAsync();
            return device;
        }

        public async Task<Device> UpdateDeviceAsync(Device device)
        {
            // Get the existing device to avoid tracking conflicts
            var existingDevice = await _context.Devices.FindAsync(device.Id);
            
            if (existingDevice == null)
                throw new ArgumentException("Device not found", nameof(device));

            // Update only the device properties, not navigation properties
            existingDevice.Name = device.Name;
            existingDevice.Code = device.Code;
            existingDevice.SerialNumber = device.SerialNumber;
            existingDevice.Description = device.Description;
            existingDevice.CategoryId = device.CategoryId;
            existingDevice.Status = device.Status;
            existingDevice.Location = device.Location;
            
            await _context.SaveChangesAsync();
            return existingDevice;
        }

        public async Task<bool> DeleteDeviceAsync(int id)
        {
            var device = await _context.Devices
                .Include(d => d.UserDevices)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (device == null)
                return false;

            // Check if device is currently assigned
            if (device.UserDevices?.Any(ud => ud.IsActive) == true)
                throw new InvalidOperationException("Cannot delete device that is currently assigned to a user.");

            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeviceExistsAsync(int id)
        {
            return await _context.Devices.AnyAsync(d => d.Id == id);
        }

        public async Task<bool> DeviceCodeExistsAsync(string code, int? excludeId = null)
        {
            var query = _context.Devices.Where(d => d.Code.ToLower() == code.ToLower());
            
            if (excludeId.HasValue)
                query = query.Where(d => d.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        /// <summary>
        /// Validates the business rule that "In Use" devices must be assigned to a user
        /// </summary>
        public async Task<string?> ValidateDeviceStatusBusinessRuleAsync(int deviceId, DeviceStatus newStatus)
        {
            var device = await _context.Devices
                .Include(d => d.UserDevices!.Where(ud => ud.IsActive))
                .FirstOrDefaultAsync(d => d.Id == deviceId);

            if (device == null)
                return "Device not found";

            var hasActiveAssignment = device.UserDevices?.Any(ud => ud.IsActive) ?? false;

            // Rule 1: If setting status to "In Use", device must be assigned to a user
            if (newStatus == DeviceStatus.InUse && !hasActiveAssignment)
            {
                return "Cannot set device status to 'In Use' without assigning it to a user first.";
            }

            // Rule 2: If device has active assignment, status should be "In Use"
            if (hasActiveAssignment && newStatus != DeviceStatus.InUse)
            {
                return $"Device is currently assigned to a user. Status should be 'In Use' or return the device first.";
            }

            return null; // No validation errors
        }
    }
}
