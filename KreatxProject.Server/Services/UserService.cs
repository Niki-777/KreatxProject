using Microsoft.AspNetCore.Identity;
using KreatxProject.Models;

namespace KreatxProject.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> UpdateProfileAsync(string userId, string fullName, IFormFile? profilePicture)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            // Perditesojme emrin e plote
            user.FullName = fullName;

            // Nese perdoruesi ka ngarkuar nje foto te re
            if (profilePicture != null && profilePicture.Length > 0)
            {
                // Krijohet folderi wwwroot/uploads nese nuk ekziston
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Gjenerojme nje emer unikal per foton qe mos te kete perplasje emrash
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(profilePicture.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Ruajme skedarin fizikisht ne server
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await profilePicture.CopyToAsync(fileStream);
                }

                // Ruajme rrugen relative ne databaze per t'u aksesuar nga React
                user.ProfilePicturePath = "/uploads/" + uniqueFileName;
            }

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<string?> GetProfilePicturePathAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user?.ProfilePicturePath;
        }
    }
}