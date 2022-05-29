using Lumeer.Models;
using Lumeer.Models.Rest;
using Lumeer.Services;
using Lumeer.Utils;
using Lumeer.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Task = System.Threading.Tasks.Task;

namespace Lumeer.ViewModels
{
    public class NotificationsViewModel : BaseViewModel
    {
        private readonly IAlertService _alertService;
        private readonly INavigationService _navigationService;

        public List<NotificationItem> OriginalNotifications { get; set; } = new List<NotificationItem>();

        public ObservableCollection<NotificationItem> DisplayedNotifications { get; set; } = new ObservableCollection<NotificationItem>();

        private string _searchedText;
        public string SearchedText
        {
            get => _searchedText;
            set
            {
                SetValue(ref _searchedText, value);

                if (string.IsNullOrEmpty(_searchedText))    // text is cleared and we have to manually show all notifications, because search is not triggered when the text is empty
                {
                    FilterDisplayedNotifications();
                }
            }
        }

        public IAsyncCommand RefreshNotificationsCmd => new AsyncCommand(RefreshNotifications);
        public ICommand SearchCmd => new Command(Search);
        public ICommand NotificationsFilterCmd => new Command(DisplayNotificationsFilter);
        public ICommand ChangeNotificationReadStatusCmd => new Command<NotificationItem>(ChangeNotificationReadStatus);
        public ICommand DeleteNotificationCmd => new Command<NotificationItem>(DeleteNotification);

        private bool _isRefreshingNotifications;
        public bool IsRefreshingNotifications
        {
            get => _isRefreshingNotifications;
            set => SetValue(ref _isRefreshingNotifications, value);
        }

        NotificationsFilterSettings _notificationsFilterSettings = new NotificationsFilterSettings();

        public NotificationsViewModel()
        {
            _alertService = DependencyService.Get<IAlertService>();
            _navigationService = DependencyService.Get<INavigationService>();

            Task.Run(RefreshNotifications);
        }

        private void DisplayNotificationsFilter()
        {
            var notificationsFilterPage = new NotificationsFilterPage(_notificationsFilterSettings);
            notificationsFilterPage.NotificationsFilterViewModel.NotificationsFilterChanged += NotificationsFilterViewModel_NotificationsFilterChanged;
            _navigationService.PushAsync(notificationsFilterPage);
        }

        private void NotificationsFilterViewModel_NotificationsFilterChanged()
        {
            FilterDisplayedNotifications();
        }

        private void Search()
        {
            FilterDisplayedNotifications();
        }

        private void FilterDisplayedNotifications()
        {
            DisplayedNotifications.Clear();

            bool shouldSearch = !string.IsNullOrEmpty(SearchedText);

            foreach (var notificationItem in OriginalNotifications)
            {
                if (_notificationsFilterSettings.OnlyUnread && notificationItem.Notification.Read)
                {
                    continue;
                }

                if (shouldSearch && !notificationItem.Title.Contains(SearchedText))
                {
                    continue;
                }

                DisplayedNotifications.Add(notificationItem);
            }
        }

        private async Task RefreshNotifications()
        {
            IsRefreshingNotifications = true;

            try
            {
                List<Notification> notifications = await ApiClient.Instance.GetNotifications();

                OriginalNotifications.Clear();

                foreach (var notification in notifications)
                {
                    var notificationItem = new NotificationItem(notification);
                    OriginalNotifications.Add(notificationItem);
                }

                FilterDisplayedNotifications();
            }
            catch (Exception ex)
            {
                var message = "Could not refresh notifications";
                await _alertService.DisplayAlert("Error", message, "Ok", ex);
            }
            finally
            {
                IsRefreshingNotifications = false;
            }
        }

        private async void ChangeNotificationReadStatus(NotificationItem notificationItem)
        {
            try
            {
                var notification = notificationItem.Notification;
                bool newReadValue = !notification.Read;
                await ApiClient.Instance.ChangeNotificationReadStatus(notification, newReadValue);
                notificationItem.ChangeReadStatus();

                if (_notificationsFilterSettings.OnlyUnread && notificationItem.Notification.Read)
                {
                    DisplayedNotifications.Remove(notificationItem);
                }
            }
            catch (Exception ex)
            {
                await _alertService.DisplayAlert("Error", "Sorry, there was an error while changing read status", "Ok", ex);
                return;
            }
        }

        private async void DeleteNotification(NotificationItem notificationItem)
        {
            bool delete = await _alertService.DisplayAlert("Warning", $"Do you really want to delete notification {notificationItem.Title}?", "Yes", "No");
            if (!delete)
            {
                return;
            }

            try
            {
                await ApiClient.Instance.DeleteNotification(notificationItem.Notification);
                DisplayedNotifications.Remove(notificationItem);
                OriginalNotifications.Remove(notificationItem);
            }
            catch (Exception ex)
            {
                await _alertService.DisplayAlert("Error", "Sorry, there was an error while deleting notification", "Ok", ex);
                return;
            }
        }
    }
}