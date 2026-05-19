using System.ComponentModel.DataAnnotations;

namespace KreatxProject.Models
{
    public class ProjectEmployee
    {
        [Required]
        public int ProjectId { get; set; }
        public virtual Project? Project { get; set; }

        [Required]
        public string EmployeeId { get; set; } = string.Empty;
        public virtual ApplicationUser? Employee { get; set; }
    }
}