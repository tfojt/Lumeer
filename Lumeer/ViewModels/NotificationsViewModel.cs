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
using Xamarin.Forms;
using Task = System.Threading.Tasks.Task;

namespace Lumeer.ViewModels
{
    public class NotificationsViewModel : BaseViewModel
    {
        private readonly IAlertService _alertService;
        private readonly INavigationService _navigationService;

        private List<Notification> _originalNotifications = new List<Notification>();
        public List<Notification> OriginalNotifications
        {
            get => _originalNotifications;
            set
            {
                _originalNotifications = value;

                FilterDisplayedNotifications();
            }
        }

        public ObservableCollection<NotificationItem> DisplayedNotifications { get; set; } = new ObservableCollection<NotificationItem>();

        private string _searchedText;
        public string SearchedText
        {
            get => _searchedText;
            set
            {
                SetValue(ref _searchedText, value);

                if (string.IsNullOrEmpty(_searchedText))    // text is cleared and we have to manually show all tasks, because search is not triggered when the text is empty
                {
                    FilterDisplayedNotifications();
                }
            }
        }

        public ICommand RefreshNotificationsCmd { get; set; }
        public ICommand SearchCmd { get; set; }
        public ICommand SearchSettingsCmd { get; set; }
        public ICommand ChangeNotificationReadStatusCmd { get; set; }

        private bool _isRefreshingNotifications;
        public bool IsRefreshingNotifications
        {
            get => _isRefreshingNotifications;
            set => SetValue(ref _isRefreshingNotifications, value);
        }

        private NotificationItem _selectedNotification;
        public NotificationItem SelectedNotification
        {
            get => _selectedNotification;
            set
            {
                SetValue(ref _selectedNotification, value);
                if (value != null)
                {
                    DisplayTaskDetail(value);
                }
            }
        }

        public NotificationsViewModel()
        {
            _alertService = DependencyService.Get<IAlertService>();
            _navigationService = DependencyService.Get<INavigationService>();

            RefreshNotificationsCmd = new Command(RefreshNotifications);
            SearchCmd = new Command(Search);
            SearchSettingsCmd = new Command(DisplaySearchSettings);
            ChangeNotificationReadStatusCmd = new Command<NotificationItem>(ChangeNotificationReadStatus);

            Task.Run(RefreshNotifications);
        }

        private async void DisplayTaskDetail(NotificationItem notificationItem)
        {
            // TODOT cache LastTaskDetail and unhook TaskChangesSaved event?

            /*var task = notificationItem.Task;
            var table = Session.Instance.AllTables.Single(t => t.Id == task.CollectionId);
            var taskDetailPage = new TaskDetailPage(task, table);
            taskDetailPage.TaskDetailViewModel.TaskChangesSaved += TaskDetailViewModel_TaskChangesSaved;

            await _navigationService.PushAsync(taskDetailPage);
            SelectedNotification = null;*/
        }

        private void TaskDetailViewModel_TaskChangesSaved(Models.Rest.Task task)
        {
            /*// TODOT make binding work
            var taskIndex = Tasks.IndexOf(task);
            Tasks.Remove(task);
            Tasks.Insert(taskIndex, task);*/
            RefreshNotifications();
        }

        private void CreateTask()
        {
            var newTaskPage = new NewTaskPage();
            newTaskPage.NewTaskViewModel.TaskCreated += NewTaskViewModel_TaskCreated;
            _navigationService.PushModalAsync(newTaskPage);
        }

        private void NewTaskViewModel_TaskCreated(Models.Rest.Task task)
        {
            /*// TODOT 
            Tasks.Add(task);*/
            RefreshNotifications();
        }

        private void DisplaySearchSettings()
        {
            _navigationService.PushAsync(new SearchSettingsPage());
            // TODOT apply settings
        }

        private void Search()
        {
            FilterDisplayedNotifications();
        }

        private void FilterDisplayedNotifications()
        {
            DisplayedNotifications.Clear();

            if (string.IsNullOrEmpty(SearchedText))
            {
                foreach (var notification in OriginalNotifications)
                {
                    var notificationItem = new NotificationItem(notification);
                    DisplayedNotifications.Add(notificationItem);
                }
            }
            else
            {
                foreach (var notification in OriginalNotifications)
                {
                    foreach (object value in notification.Data.Values)
                    {
                        if (value == null)
                        {
                            continue;
                        }

                        string stringValue = value.ToString();
                        if (stringValue.Contains(SearchedText))
                        {
                            var notificationItem = new NotificationItem(notification);
                            DisplayedNotifications.Add(notificationItem);
                            break;
                        }
                    }
                }
            }
        }

        private async void RefreshNotifications()
        {
            IsRefreshingNotifications = true;

            try
            {
                OriginalNotifications = await ApiClient.Instance.GetNotifications();
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
            /*try
            {
                var task = notificationItem.Task;
                bool newFavoriteValue = !task.Favorite;
                await ApiClient.Instance.ChangeTaskFavoriteStatus(task, newFavoriteValue);
                task.Favorite = newFavoriteValue;
            }
            catch (Exception ex)
            {
                await _alertService.DisplayAlert("Error", "Sorry, there was an error while changing favorite status", "Ok", ex);
                return;
            }*/
        }
    }
}