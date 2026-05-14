using Microsoft.AspNetCore.Identity;

namespace KreatxProject.Models
{
    // Kjo klase trashegon (inherits) nga IdentityUser
    public class ApplicationUser : IdentityUser
    {
        public string? ProfilePicture { get; set; }
    }
}