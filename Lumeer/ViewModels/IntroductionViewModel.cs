using Lumeer.Models;
using Lumeer.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Lumeer.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using Lumeer.Utils;

namespace Lumeer.ViewModels
{
    public class IntroductionViewModel
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IAlertService _alertService;

        public ICommand AuthenticateCmd { get; set; }

        public ObservableCollection<IntroductionTip> IntroductionTips { get; set; }

        public IntroductionViewModel()
        {
            _authenticationService = DependencyService.Get<IAuthenticationService>();
            _alertService = DependencyService.Get<IAlertService>();

            AuthenticateCmd = new Command(Authenticate);

            IntroductionTips = new ObservableCollection<IntroductionTip>
            {
                new IntroductionTip("Lumeer.Images.kanban.gif", "Tip 0"),
                new IntroductionTip("Lumeer.Images.lumeerLogo.png", "Tip 1"),
                new IntroductionTip("Lumeer.Images.lumeerLogo.png", "Tip 2"),
                new IntroductionTip("Lumeer.Images.lumeerLogo.png", "Tip 3"),
            };
        }

        private async void Authenticate()
        {
            var authenticationResult = await _authenticationService.Authenticate();
            if (!authenticationResult.IsError)
            {
                await SecureStorage.SetAsync("accessToken", authenticationResult.AccessToken);
                await SecureStorage.SetAsync("identityToken", authenticationResult.IdToken);
                ApiClient.Instance.Authorize(authenticationResult.AccessToken);
                App.Current.MainPage = new NavigationPage(new MainPage());
            }
            else
            {
                await _alertService.DisplayAlert("Error", $"{authenticationResult.Error}", "Ok");
            }
        }
    }
}
