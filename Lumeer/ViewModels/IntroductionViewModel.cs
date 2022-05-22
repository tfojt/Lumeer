using Lumeer.Models;
using Lumeer.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Lumeer.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using Lumeer.Utils;
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Lumeer.ViewModels
{
    public class IntroductionViewModel
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IAlertService _alertService;
        private readonly INavigationService _navigationService;
        private readonly ISecureStorageService _secureStorageService;

        public ICommand AuthenticateCmd { get; set; }

        public ObservableCollection<IntroductionTip> IntroductionTips { get; set; }

        public IntroductionViewModel()
        {
            _authenticationService = DependencyService.Get<IAuthenticationService>();
            _alertService = DependencyService.Get<IAlertService>();
            _navigationService = DependencyService.Get<INavigationService>();
            _secureStorageService = DependencyService.Get<ISecureStorageService>();

            AuthenticateCmd = new AsyncCommand(Authenticate, allowsMultipleExecutions: false);

            IntroductionTips = new ObservableCollection<IntroductionTip>
            {
                new IntroductionTip("Lumeer.Images.kanban.gif", "Tip 0"),
                new IntroductionTip("Lumeer.Images.lumeerLogo.png", "Tip 1"),
                new IntroductionTip("Lumeer.Images.lumeerLogo.png", "Tip 2"),
                new IntroductionTip("Lumeer.Images.lumeerLogo.png", "Tip 3"),
            };
        }

        private async Task Authenticate()
        {
            using (new LoadingPopup())
            {
                string accessToken = await _secureStorageService.GetAsync(SecureStorageService.ACCESS_TOKEN_KEY);
                if (accessToken == null)
                {
                    accessToken = await Login();
                    if (accessToken == null)
                    {
                        return;
                    }
                }
                else
                {
                    ApiClient.Instance.Authorize(accessToken);
                    bool isValid = await ApiClient.Instance.IsAccessTokenValid();
                    if (!isValid)
                    {
                        _secureStorageService.Remove(SecureStorageService.ACCESS_TOKEN_KEY);
                        accessToken = await Login();
                        if (accessToken == null)
                        {
                            return;
                        }
                    }
                }

                ApiClient.Instance.Authorize(accessToken);

                try
                {
                    await Session.Instance.LoadUsersInitialData();
                    App.Current.MainPage = new NavigationPage(new MainPage());
                }
                catch (Exception ex)
                {
                    var message = "Could not obtain data from server";
                    await _alertService.DisplayAlert("Error", message, "Ok", ex);
                }
            }
        }

        private async Task<string> Login()
        {
            var authenticationResult = await _authenticationService.Authenticate();
            if (authenticationResult.IsError)
            {
                await _alertService.DisplayAlert("Error", $"{authenticationResult.Error}", "Ok");
                return null;
            }

            await _secureStorageService.SetAsync(SecureStorageService.ACCESS_TOKEN_KEY, authenticationResult.AccessToken);
            return authenticationResult.AccessToken;
        }
    }
}
