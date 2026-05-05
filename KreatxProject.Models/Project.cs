using System.ComponentModel.DataAnnotations;

namespace KreatxProject.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Emri i projektit është i detyrueshëm")] // Validim sipas dokumentit
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty; // Rregullon Warning CS8618

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Now;

        public DateTime? EndDate { get; set; }

        // Lidhja me Task-et: Një projekt mund të ketë shumë detyra
        public virtual ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    }
}