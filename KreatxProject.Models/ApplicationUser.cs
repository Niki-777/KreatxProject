using Microsoft.AspNetCore.Identity;

namespace KreatxProject.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Emri i plote i perdoruesit
        public string FullName { get; set; } = string.Empty;

        // Fusha e re per foton e profilit
        public string? ProfilePicturePath { get; set; }
    }
}