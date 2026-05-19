using KreatxProject.Models;
using KreatxProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace KreatxProject.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Tani te gjithe te loguarit hyjne ne kontrollor, por ndajme rolet me poshte
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserService _userService; // Injektojme sherbimin e ri te profilit

        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserService userService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = userService;
        }

        // GET: api/users
        // Vetem Administratori ka qasje ketu per te listuar perdoruesit
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userListWithRoles = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userListWithRoles.Add(new
                {
                    user.Id,
                    user.Email,
                    Role = roles.FirstOrDefault() ?? "Pa Rol"
                });
            }

            return Ok(userListWithRoles);
        }

        // POST: api/users
        // Vetem Administratori mund te kijoje perdorues te rinj
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateUser([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null) return BadRequest(new { message = "Ky email eshte i regjistruar nje here ne sistem." });

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { message = string.Join(" ", errors) });
            }

            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                await _roleManager.CreateAsync(new IdentityRole(model.Role));
            }

            await _userManager.AddToRoleAsync(user, model.Role);

            return Ok(new { message = $"Perdoruesi u krijua me sukses me rolin {model.Role}!" });
        }

       [HttpPost]
        // POST: api/users/profile
        // Perdoret FromForm sepse po ngarkojme skedar (Multipart/Form-Data)
        [HttpPost("profile")]
        public async Task<IActionResult> UpdateProfile([FromForm] string fullName, IFormFile? profilePicture)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _userService.UpdateProfileAsync(userId, fullName, profilePicture);
            if (!result) return BadRequest(new { message = "Nuk u mundesua perditesimi i profilit." });

            return Ok(new { message = "Profili u perditesua me sukses." });
        }

        // GET: api/users/profile-picture
        [HttpGet("profile-picture")]
        public async Task<IActionResult> GetProfilePicture()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var path = await _userService.GetProfilePicturePathAsync(userId);
            return Ok(new { path = path });
        }
    }

    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Employee";
    }
}