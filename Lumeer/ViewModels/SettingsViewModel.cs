using Lumeer.Services;
using Lumeer.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Lumeer.ViewModels
{
    public class SettingsViewModel
    {
        public IAsyncCommand GetHelpCmd => new AsyncCommand(GetHelp, allowsMultipleExecutions: false);
        public IAsyncCommand LogoutCmd => new AsyncCommand(Logout, allowsMultipleExecutions: false);

        private readonly IAlertService _alertService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ISecureStorageService _secureStorageService;

        public SettingsViewModel()
        {
            _alertService = DependencyService.Get<IAlertService>();
            _authenticationService = DependencyService.Get<IAuthenticationService>();
            _secureStorageService = DependencyService.Get<ISecureStorageService>();
        }

        private async Task GetHelp()
        {
            string uri = "https://www.lumeer.io/get-help/";

            try
            {
                await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                var message = "Could not load help page.";
                await _alertService.DisplayAlert("Error", message, "Ok", ex);
            }
        }

        private async Task Logout()
        {
            await _authenticationService.Logout();

            _secureStorageService.Remove(SecureStorageService.ACCESS_TOKEN_KEY);

            App.Current.MainPage = new IntroductionPage();
        }
    }
}
