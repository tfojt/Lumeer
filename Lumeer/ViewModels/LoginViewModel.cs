using Lumeer.Auth;
using System.Windows.Input;
using Lumeer.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Lumeer.ViewModels
{
    public class LoginViewModel
    {
        private readonly IAuthenticationService _authenticationService;

        public ICommand LoginCmd { get; set; }
        public ICommand LogoutCommand { get; }

        public LoginViewModel()
        {
            _authenticationService = DependencyService.Get<IAuthenticationService>();
            LoginCmd = new Command(Login);
            LogoutCommand = new Command(Logout);
        }

        private async void Login()
        {
            var authenticationResult = await _authenticationService.Authenticate();
            if (!authenticationResult.IsError)
            {
                await SecureStorage.SetAsync("accessToken", authenticationResult.AccessToken);
                //IsLoggedIn = true;
            }

            //await Shell.Current.GoToAsync($"//{nameof(BooksPage)}");
        }

        private async void Logout()
        {
            await _authenticationService.Logout();
            SecureStorage.Remove("accessToken");
            //IsLoggedIn = false;
        }
    }
}
