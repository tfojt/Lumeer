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
                new IntroductionTip("undraw_schedule_pnbk.png", "Keep all your tasks in the same place", "Lumeer’s task management software is the one source of truth for your team tasks. No more switching between endless excel files!"),
                new IntroductionTip("undraw_data_xmfy.png", "Track your team’s progress", "Who is available? How many tasks have been completed? Do we meet the deadline? It’s all in Lumeer work management platform."),
                new IntroductionTip("undraw_booking_33fn.png", "Set due dates and never miss a deadline", "Manage your weekly tasks, receive notifications two days before that important presentation is coming up and receive an automated email when a task is complete."),
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
