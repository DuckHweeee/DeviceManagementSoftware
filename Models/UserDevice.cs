using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeviceManagementSoftware.Models
{
    public class UserDevice
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "User")]
        public int UserId { get; set; }

        [Required]
        [Display(Name = "Device")]
        public int DeviceId { get; set; }

        [Display(Name = "Assigned Date")]
        public DateTime AssignedDate { get; set; } = DateTime.Now;

        [Display(Name = "Return Date")]
        public DateTime? ReturnDate { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? Notes { get; set; }

        [Display(Name = "Is Active Assignment")]
        public bool IsActive { get; set; } = true;

        // Foreign Keys
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("DeviceId")]
        public virtual Device? Device { get; set; }
    }
}
