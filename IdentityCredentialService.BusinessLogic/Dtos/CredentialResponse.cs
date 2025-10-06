namespace IdentityCredentialService.BusinessLogic.Dtos
{
    public class CredentialResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public UserDto User { get; set; } = new();
    }
}