using IdentityCredentialService.BusinessLogic.Dtos;
using IdentityCredentialService.BusinessLogic.Services.Interfaces;
using IdentityCredentialService.Domain.Models;
using IdentityCredentialService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IdentityCredentialService.BusinessLogic.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IJwtService _jwtService;

        public UserService(AppDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<ApiResponse<string>> CreateUserAsync(RegisterRequest registerRequest)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(registerRequest.FirstName))
                    return ApiResponse<string>.BadRequest("First name is required.");

                if (string.IsNullOrWhiteSpace(registerRequest.LastName))
                    return ApiResponse<string>.BadRequest("Last name is required.");

                if (string.IsNullOrWhiteSpace(registerRequest.Email))
                    return ApiResponse<string>.BadRequest("Email is required.");

                if (string.IsNullOrWhiteSpace(registerRequest.Password))
                    return ApiResponse<string>.BadRequest("Password is required.");

                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == registerRequest.Email.ToLower());

                if (existingUser != null)
                    return ApiResponse<string>.BadRequest("User with this email already exists.");

                var hashedPassword = PasswordHelper.HashPassword(registerRequest.Password);
                
                var newUser = new User
                {
                    FirstName = registerRequest.FirstName,
                    LastName = registerRequest.LastName,
                    Email = registerRequest.Email,
                    Password = hashedPassword
                };
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return ApiResponse<string>.Ok(newUser.Id, "User created successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.InternalServerError($"Failed to create user: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CredentialResponse>> IssueCredentialAsync(IssueCredentialRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email))
                    return ApiResponse<CredentialResponse>.BadRequest("Email is required.");

                if (string.IsNullOrWhiteSpace(request.Password))
                    return ApiResponse<CredentialResponse>.BadRequest("Password is required.");

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

                if (user == null)
                    return ApiResponse<CredentialResponse>.BadRequest("Invalid email or password.");
                
                if (!PasswordHelper.VerifyPassword(request.Password, user.Password))
                    return ApiResponse<CredentialResponse>.BadRequest("Invalid email or password.");
                var claims = new List<Claim>
                        {   new Claim(ClaimTypes.NameIdentifier,user.Id),
                            new Claim("email", user?.Email)

                    };

                var accessToken = _jwtService.GenerateAccessToken(claims);
                var refreshToken = _jwtService.GenerateRefreshToken();

                var response = new CredentialResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    User = new UserDto
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email
                    }
                };

                return ApiResponse<CredentialResponse>.Ok(response, "Credential issued successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<CredentialResponse>.InternalServerError($"Failed to issue credential: {ex.Message}");
            }
        }
    }
}
