using IdentityCredentialService.Domain.Models;
using System.Security.Claims;

namespace IdentityCredentialService.BusinessLogic.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
    }
}