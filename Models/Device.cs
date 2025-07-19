using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeviceManagementSoftware.Models
{
    public enum DeviceStatus
    {
        [Display(Name = "In Use")]
        InUse = 1,
        
        [Display(Name = "Broken")]
        Broken = 2,
        
        [Display(Name = "Under Maintenance")]
        UnderMaintenance = 3,
        
        [Display(Name = "Available")]
        Available = 4
    }

    public class Device
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Device name is required")]
        [StringLength(200, ErrorMessage = "Device name cannot exceed 200 characters")]
        [Display(Name = "Device Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Device code is required")]
        [StringLength(50, ErrorMessage = "Device code cannot exceed 50 characters")]
        [Display(Name = "Device Code")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a device category")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Please select device status")]
        public DeviceStatus Status { get; set; } = DeviceStatus.Available;

        [Display(Name = "Date of Entry")]
        public DateTime DateOfEntry { get; set; } = DateTime.Now;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
        public string? Location { get; set; }

        [StringLength(100, ErrorMessage = "Serial number cannot exceed 100 characters")]
        [Display(Name = "Serial Number")]
        public string? SerialNumber { get; set; }

        // Foreign Key
        [ForeignKey("CategoryId")]
        public virtual DeviceCategory? Category { get; set; }

        // Navigation property for user assignments
        public virtual ICollection<UserDevice>? UserDevices { get; set; }
    }
}
