using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using KreatxProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace KreatxProject.Server.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<AuthResult> LoginAsync(LoginModel model)
        {
            // 1. Kontrollojmë nëse përdoruesi ekziston në databazë përmes Email-it
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Email ose Password i gabuar."
                };
            }

            // 2. Marrim rolet e përdoruesit (p.sh. Administrator ose Employee)
            var userRoles = await _userManager.GetRolesAsync(user);

            // 3. Krijojmë "Claims" (të dhënat identifikuese që do të mbajë Token-i)
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            // Shtojmë rolet tek Claims që React-i të dijë çfarë roli ka ky përdorues
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            // 4. Lexojmë konfigurimet e JWT nga appsettings.json
            var jwtSettings = _configuration.GetSection("Jwt");
            var keyStr = jwtSettings["Key"] ?? "Key_E_Sigurise_Kreatx_2026_Sekret_Super_E_Gjate";
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));

            // 5. Gjenerojmë Token-in JWT që do të jetë i vlefshëm për 3 orë
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            // 6. Kthejmë rezultatin me sukses dhe token-in e gjeneruar
            return new AuthResult
            {
                IsSuccess = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo,
                Role = userRoles.FirstOrDefault() ?? "Employee"
            };
        }
    }
}