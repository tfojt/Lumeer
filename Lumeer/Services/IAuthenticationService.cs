using System.Threading.Tasks;
using Lumeer.Auth;

namespace Lumeer.Services
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> Authenticate();
        AuthenticationResult AuthenticationResult { get; }
        Task Logout();
    }
}
