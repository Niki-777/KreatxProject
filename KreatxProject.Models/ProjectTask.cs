using System.ComponentModel.DataAnnotations;

namespace KreatxProject.Models
{
    public class ProjectTask
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Titulli i task-ut është i detyrueshëm")] // Validim sipas dokumentit
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty; // Rregullon Warning CS8618

        public string? Description { get; set; }

        public bool IsCompleted { get; set; } = false; // Punonjësi mund ta shënojë si të përfunduar[cite: 1]

        // Foreign Key: Lidhim task-un me projektin përkatës
        [Required]
        public int ProjectId { get; set; }

        // Navigimi: Ky rresht lejon EF të marrë të dhënat e projektit prind
        public virtual Project Project { get; set; } = default!; // Rregullon Warning CS8618
    }
}