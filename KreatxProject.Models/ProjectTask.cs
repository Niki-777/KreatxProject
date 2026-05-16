using System.ComponentModel.DataAnnotations;

namespace KreatxProject.Models
{
    public class ProjectTask
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Titulli i task-ut është i detyrueshëm")]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsCompleted { get; set; } = false;

        // Foreign Key: Lidhim task-un me projektin përkatës
        [Required]
        public int ProjectId { get; set; }

        public virtual Project? Project { get; set; }

        // Foreign Key për Punonjësin e caktuar (Assigned To)
        public string? AssignedToUserId { get; set; }

        public virtual ApplicationUser? AssignedToUser { get; set; }
    }
}