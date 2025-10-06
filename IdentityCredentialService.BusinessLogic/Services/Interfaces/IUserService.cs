using IdentityCredentialService.BusinessLogic.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityCredentialService.BusinessLogic.Services.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<string>> CreateUserAsync(RegisterRequest registerRequest);
        Task<ApiResponse<CredentialResponse>> IssueCredentialAsync(IssueCredentialRequest request);
    }
}
