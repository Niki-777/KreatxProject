using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace KreatxProject.Services
{
    public interface IUserService
    {
        Task<bool> UpdateProfileAsync(string userId, string fullName, IFormFile? profilePicture);
        Task<string?> GetProfilePicturePathAsync(string userId);
    }
}