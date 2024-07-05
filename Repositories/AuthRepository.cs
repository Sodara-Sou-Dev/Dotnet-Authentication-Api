using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using cube_gaming_store_back.Data;
using cube_gaming_store_back.DTOs.Posts;
using cube_gaming_store_back.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using static cube_gaming_store_back.DTOs.GeneralResponse;

namespace cube_gaming_store_back.Repositories
{
    public class AuthRepository(UserManager<ApplicationUser> userManager,
                                    RoleManager<IdentityRole> roleManager,
                                    SignInManager<ApplicationUser> signInManager,
                                    IConfiguration config,
                                    AppDbContext context) : IAuth
    {
        public async Task<bool> FindUserByEmail(string email)
        {
            var userExist = await userManager.FindByEmailAsync(email);
            return userExist != null;
        }

        public async Task<bool> FindUserById(string userId)
        {
            var userExist = await context.ApplicationUsers.AnyAsync(u => u.Id == userId);
            return userExist;
        }

        public async Task<SignUpResponse> SignUp(SignUpPostDTO user)
        {
            var newUser = new ApplicationUser()
            {
                UserName = user.FirstName + user.LastName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PasswordHash = user.Password,
            };
            var createUser = await userManager.CreateAsync(newUser!, user.Password!);
            if (!createUser.Succeeded)
            {
                var errorDescriptions = createUser.Errors.Select(error => error.Description);
                var errorMessage = string.Join(" ", errorDescriptions);
                Console.Error.WriteLine(errorMessage);
                return new SignUpResponse(false, errorMessage);
            }
            if (user.Role == "User")
            {
                await userManager.AddToRoleAsync(newUser, "User");
                return new SignUpResponse(true, "Account successfully created");
            }
            else
            {
                await userManager.AddToRoleAsync(newUser, "Admin");
                return new SignUpResponse(true, "Account successfully created");
            }
        }

        public async Task<SignInResponse> SignIn(SignInPostDTO user)
        {
            var getUser = await userManager.FindByEmailAsync(user.Email!);
            if (getUser is null)
            {
                return new SignInResponse(null!, "User not found");
            }
            var result = await signInManager.PasswordSignInAsync(getUser, user.Password!, isPersistent: false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                return new SignInResponse(null!, "Invalid password");
            }
            var getUserRole = await userManager.GetRolesAsync(getUser);
            var userSession = new UserSession(getUser.Id, getUser.FirstName, getUser.LastName, getUser.Email, getUserRole.First());
            string jwtToken = GenerateToken(userSession);
            return new SignInResponse(jwtToken!, "Login completed");
        }

        public async Task<bool> CreateRole(string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new IdentityRole(roleName);
                await roleManager.CreateAsync(role);
                return true;
            }
            return false;
        }

        public async Task<bool> SignOut()
        {
            await signInManager.SignOutAsync();
            return true;
        }

        private string GenerateToken(UserSession user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id!),
                new Claim(ClaimTypes.Name, user.FirstName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, user.Role!)
            };
            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Issuer"],
                claims: userClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public JwtSecurityToken DecodeJwtToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken;
        }
    }
}
