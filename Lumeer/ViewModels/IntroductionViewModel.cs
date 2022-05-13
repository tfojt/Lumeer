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

        public ICommand AuthenticateCmd { get; set; }

        public ObservableCollection<IntroductionTip> IntroductionTips { get; set; }

        public IntroductionViewModel()
        {
            _authenticationService = DependencyService.Get<IAuthenticationService>();
            _alertService = DependencyService.Get<IAlertService>();
            _navigationService = DependencyService.Get<INavigationService>();

            AuthenticateCmd = new AsyncCommand(Authenticate, allowsMultipleExecutions:false);

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
            const string ACCESS_TOKEN_KEY = "accessToken";

            using (new LoadingPopup())
            {
                string accessToken = await GetToken(ACCESS_TOKEN_KEY);
                if (accessToken == null)
                {
                    var authenticationResult = await _authenticationService.Authenticate();
                    if (authenticationResult.IsError)
                    {
                        await _alertService.DisplayAlert("Error", $"{authenticationResult.Error}", "Ok");
                        return;
                    }

                    accessToken = authenticationResult.AccessToken;
                    await SaveToken(ACCESS_TOKEN_KEY, accessToken);
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

        private async Task<string> GetToken(string key)
        {
            try
            {
                return await SecureStorage.GetAsync(key);
            }
            catch (Exception ex)
            {
                // Possible that device doesn't support secure storage on device.
                Debug.WriteLine(ex);
                return null;
            }
        }

        private async Task SaveToken(string key, string token)
        {
            try
            {
                await SecureStorage.SetAsync(key, token);
            }
            catch (Exception ex)
            {
                // Possible that device doesn't support secure storage on device.
                Debug.WriteLine(ex);
            }
        }
    }
}
