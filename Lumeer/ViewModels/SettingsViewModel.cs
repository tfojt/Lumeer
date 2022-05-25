using Lumeer.Services;
using Lumeer.Utils;
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
        public IAsyncCommand ChangeLanguageCmd => new AsyncCommand(ChangeLanguage, allowsMultipleExecutions: false);
        public IAsyncCommand DisplayNotificationsSettingsCmd => new AsyncCommand(DisplayNotificationsSettings, allowsMultipleExecutions: false);
        public IAsyncCommand GetHelpCmd => new AsyncCommand(GetHelp, allowsMultipleExecutions: false);
        public IAsyncCommand LogoutCmd => new AsyncCommand(Logout, allowsMultipleExecutions: false);

        public string UserName { get; set; }
        public string UserEmail { get; set; }

        private readonly IAlertService _alertService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ISecureStorageService _secureStorageService;
        private readonly INavigationService _navigationService;

        public SettingsViewModel()
        {
            _alertService = DependencyService.Get<IAlertService>();
            _authenticationService = DependencyService.Get<IAuthenticationService>();
            _secureStorageService = DependencyService.Get<ISecureStorageService>();
            _navigationService = DependencyService.Get<INavigationService>();

            UserName = Session.Instance.User.Name;
            UserEmail = Session.Instance.User.Email;
        }

        private async Task ChangeLanguage()
        {
            var languagesPage = new LanguagesPage();
            await _navigationService.PushAsync(languagesPage);
        }
        
        private async Task DisplayNotificationsSettings()
        {
            var notificationsSettingsPage = new NotificationsSettingsPage();
            await _navigationService.PushAsync(notificationsSettingsPage);
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
