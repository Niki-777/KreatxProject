using System.Threading.Tasks;
using KreatxProject.Models;

namespace KreatxProject.Server.Services
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(LoginModel model);
    }
}