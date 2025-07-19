using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DeviceManagementSoftware.Models.ViewModels
{
    public class DeviceSearchFilterViewModel
    {
        [Display(Name = "Search by Name or Code")]
        public string? SearchTerm { get; set; }

        [Display(Name = "Category")]
        public int? CategoryId { get; set; }

        [Display(Name = "Status")]
        public DeviceStatus? Status { get; set; }

        [Display(Name = "Location")]
        public string? Location { get; set; }

        // For dropdowns
        public SelectList? Categories { get; set; }
        public SelectList? Statuses { get; set; }

        // Results
        public IEnumerable<Device>? Devices { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

    public class DeviceCreateEditViewModel
    {
        public Device Device { get; set; } = new Device();
        public SelectList? Categories { get; set; }
        public SelectList? Statuses { get; set; }
        public User? AssignedUser { get; set; }
        public UserDevice? CurrentAssignment { get; set; }
        public SelectList? AvailableUsers { get; set; }
        
        // For assignment management
        public int? SelectedUserId { get; set; }
        public string? AssignmentNotes { get; set; }
    }

    public class DeviceDetailsViewModel
    {
        public Device Device { get; set; } = new Device();
        public User? AssignedUser { get; set; }
        public IEnumerable<UserDevice>? AssignmentHistory { get; set; }
    }
}
