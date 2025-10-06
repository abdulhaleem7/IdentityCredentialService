namespace IdentityCredentialService.BusinessLogic.Dtos
{
    public class IssueCredentialRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}