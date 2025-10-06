using IdentityCredentialService.BusinessLogic.Dtos;
using IdentityCredentialService.BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IdentityCredentialService.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<IdentityController> _logger;

        public IdentityController(IUserService userService, ILogger<IdentityController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <remarks>
        /// **Sample request:**
        /// 
        ///     POST /api/Identity/register
        ///     {
        ///         "FirstName": "John",
        ///         "LastName": "Doe",
        ///         "Email": "john.doe@gmail.com",
        ///         "Password": "StrongPassword123!"
        ///     }
        /// 
        /// This endpoint hashes the provided password before saving the user record.
        /// </remarks>
        /// <param name="request">The registration request payload containing user details.</param>
        /// <returns>
        /// Returns an <see cref="ApiResponse{T}"/> containing the user ID if registration succeeds.
        /// </returns>
        /// <response code="200">User account created successfully.</response>
        /// <response code="400">Invalid input or registration failed.</response>
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<string>>> Register([FromBody] RegisterRequest request)
        {
            _logger.LogInformation("User registration attempt for email: {Email}", request.Email);

            var result = await _userService.CreateUserAsync(request);

            if (!result.Success)
            {
                _logger.LogWarning("User registration failed for email: {Email}. Reason: {Message}",
                    request.Email, result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("User registered successfully with ID: {UserId}", result.Data);
            return Ok(result);
        }

        /// <summary>
        /// Issues a credential (JWT) for an existing user after validating their credentials.
        /// </summary>
        /// <remarks>
        /// **Sample request:**
        /// 
        ///     POST /api/Identity/issue-credential
        ///     {
        ///         "Email": "john.doe@gmail.com",
        ///         "Password": "StrongPassword123!"
        ///     }
        /// 
        /// This endpoint authenticates the user using their email and password.
        /// If the credentials are valid, a signed JWT credential is issued and returned.
        /// </remarks>
        /// <param name="request">
        /// The credential issuance request containing the user's email and password.
        /// </param>
        /// <returns>
        /// Returns an <see cref="ApiResponse{T}"/> containing the issued JWT token if successful.
        /// </returns>
        /// <response code="200">Credential issued successfully.</response>
        /// <response code="400">Invalid request or incorrect login credentials.</response>
        /// <response code="401">Unauthorized – when email or password is invalid.</response>
        [HttpPost("issue-credential")]
        public async Task<ActionResult<ApiResponse<CredentialResponse>>> IssueCredential([FromBody] IssueCredentialRequest request)
        {
            _logger.LogInformation("Credential issuance attempt for email: {Email}", request.Email);

            var result = await _userService.IssueCredentialAsync(request);

            if (!result.Success)
            {
                _logger.LogWarning("Credential issuance failed for email: {Email}. Reason: {Message}",
                    request.Email, result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("Credential issued successfully for email: {Email}", request.Email);
            return Ok(result);
        }

    }
}
