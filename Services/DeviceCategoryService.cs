using Microsoft.EntityFrameworkCore;
using DeviceManagementSoftware.Data;
using DeviceManagementSoftware.Models;

namespace DeviceManagementSoftware.Services
{
    public class DeviceCategoryService : IDeviceCategoryService
    {
        private readonly ApplicationDbContext _context;

        public DeviceCategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DeviceCategory>> GetAllCategoriesAsync()
        {
            return await _context.DeviceCategories
                .Include(c => c.Devices)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<DeviceCategory?> GetCategoryByIdAsync(int id)
        {
            return await _context.DeviceCategories
                .Include(c => c.Devices)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<DeviceCategory> CreateCategoryAsync(DeviceCategory category)
        {
            category.DateCreated = DateTime.Now;
            _context.DeviceCategories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<DeviceCategory> UpdateCategoryAsync(DeviceCategory category)
        {
            _context.DeviceCategories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.DeviceCategories
                .Include(c => c.Devices)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return false;

            // Check if category has devices
            if (category.Devices.Any())
                throw new InvalidOperationException("Cannot delete category that has devices assigned to it.");

            _context.DeviceCategories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CategoryExistsAsync(int id)
        {
            return await _context.DeviceCategories.AnyAsync(c => c.Id == id);
        }

        public async Task<bool> CategoryNameExistsAsync(string name, int? excludeId = null)
        {
            var query = _context.DeviceCategories.Where(c => c.Name.ToLower() == name.ToLower());
            
            if (excludeId.HasValue)
                query = query.Where(c => c.Id != excludeId.Value);

            return await query.AnyAsync();
        }
    }
}
