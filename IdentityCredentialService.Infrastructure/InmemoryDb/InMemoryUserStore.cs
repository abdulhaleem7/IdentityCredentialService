using IdentityCredentialService.Domain.Models;

namespace IdentityCredentialService.Infrastructure.InmemoryDb
{
    public static class InMemoryUserStore
    {
        public static List<User> Users => new List<User>();
    }
}
