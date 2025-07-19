using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DeviceManagementSoftware.Models.ViewModels
{
    public class UserSearchFilterViewModel
    {
        [Display(Name = "Search by Name or Email")]
        public string? SearchTerm { get; set; }

        [Display(Name = "Department")]
        public string? Department { get; set; }

        [Display(Name = "Show Active Only")]
        public bool ActiveOnly { get; set; } = true;

        // Results
        public IEnumerable<User>? Users { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

    public class UserDetailsViewModel
    {
        public User User { get; set; } = new User();
        public IEnumerable<UserDevice>? AssignedDevices { get; set; }
        public IEnumerable<UserDevice>? DeviceHistory { get; set; }
    }

    public class AssignDeviceViewModel
    {
        [Required]
        [Display(Name = "User")]
        public int UserId { get; set; }

        [Required]
        [Display(Name = "Device")]
        public int DeviceId { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? Notes { get; set; }

        public SelectList? Users { get; set; }
        public SelectList? AvailableDevices { get; set; }
    }
}
