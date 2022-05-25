using Lumeer.Models;
using Lumeer.Models.Rest;
using Lumeer.Models.Rest.Enums;
using Lumeer.Services;
using Lumeer.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Lumeer.ViewModels
{
    public class NotificationsSettingsViewModel : BaseViewModel
    {
        public ObservableCollection<NotificationSettingsItem> NotificationsSettings { get; set; } = new ObservableCollection<NotificationSettingsItem>();

        public ICommand ToggleSettingsCmd => new Command<NotificationSettingsItem>(ToggleSettings);

        private Dictionary<NotificationSettingsItem, bool> _initialSettings = new Dictionary<NotificationSettingsItem, bool>();

        private readonly IAlertService _alertService;

        public NotificationsSettingsViewModel()
        {
            _alertService = DependencyService.Get<IAlertService>();

            var emailNotificationsSettings = Session.Instance.User.Notifications.Settings
                .Where(s => s.NotificationChannel == NotificationChannel.Email)
                .ToList();

            var resourceShared = new NotificationSettingsItem("Resource Shared", "Organization, Project, View or Table shared", 
                NotificationType.ORGANIZATION_SHARED, NotificationType.PROJECT_SHARED, NotificationType.COLLECTION_SHARED, NotificationType.VIEW_SHARED);
            
            NotificationsSettings.Add(resourceShared);
            
            var taskAssigned = new NotificationSettingsItem("Task Assigned", "Task assigned or unassigned", 
                NotificationType.TASK_ASSIGNED, NotificationType.TASK_UNASSIGNED, NotificationType.TASK_REOPENED);

            NotificationsSettings.Add(taskAssigned);
            
            var taskUpdated = new NotificationSettingsItem("Task Updated", "Task details are updated", NotificationType.TASK_UPDATED);

            NotificationsSettings.Add(taskUpdated);
            
            var taskRemoved = new NotificationSettingsItem("Task Removed", "Your task is deleted", NotificationType.TASK_REMOVED);

            NotificationsSettings.Add(taskRemoved);
            
            var dueDateUpdates = new NotificationSettingsItem("Due Date Updates", "Due date approaching or changed",
                NotificationType.DUE_DATE_SOON, NotificationType.DUE_DATE_CHANGED, NotificationType.PAST_DUE_DATE);

            NotificationsSettings.Add(dueDateUpdates);
            
            var stateUpdated = new NotificationSettingsItem("State Updated", "Task state is updated", NotificationType.STATE_UPDATE);

            NotificationsSettings.Add(stateUpdated);
            
            var comments = new NotificationSettingsItem("Comments", "Comments on your tasks, or someone mentions you in a comment",
                NotificationType.TASK_COMMENTED, NotificationType.TASK_MENTIONED);

            NotificationsSettings.Add(comments);

            foreach (var notificationSettingsItem in NotificationsSettings)
            {
                if (emailNotificationsSettings.Any(ns => ns.NotificationType == notificationSettingsItem.NotificationTypes.First()))
                {
                    notificationSettingsItem.IsToggled = true;
                }

                _initialSettings.Add(notificationSettingsItem, notificationSettingsItem.IsToggled);
            }
        }

        private void ToggleSettings(NotificationSettingsItem notificationSettingsItem)
        {
            notificationSettingsItem.IsToggled = !notificationSettingsItem.IsToggled;
        }

        public async void CheckSettingsChanged()
        {
            bool notificationsChanged = CheckNotificationsChanged();
            if (notificationsChanged)
            {
                try
                {
                    await ApiClient.Instance.ChangeNotificationsSettings(Session.Instance.User);
                }
                catch (Exception ex)
                {
                    await _alertService.DisplayAlert("Error", "Sorry, there was an error while changing notifications settings.", "Ok", ex);
                }
            }
        }

        private bool CheckNotificationsChanged()
        {
            bool changed = false;

            foreach (var notificationSettingsItem in NotificationsSettings)
            {
                bool currentlyToggled = notificationSettingsItem.IsToggled;
                bool initiallyToggled = _initialSettings[notificationSettingsItem];

                if (initiallyToggled == currentlyToggled)
                {
                    continue;
                }

                changed = true;

                foreach (var notificationType in notificationSettingsItem.NotificationTypes)
                {
                    if (currentlyToggled)
                    {
                        var notificationSettings = new NotificationSettings
                        {
                            NotificationType = notificationType,
                            NotificationChannel = NotificationChannel.Email,
                            NotificationFrequency = NotificationFrequency.Immediately
                        };

                        Session.Instance.User.Notifications.Settings.Add(notificationSettings);
                    }
                    else
                    {
                        var notificationSettings = Session.Instance.User.Notifications.Settings
                        .Where(s => s.NotificationChannel == NotificationChannel.Email)
                        .Single(s => s.NotificationType == notificationType);

                        Session.Instance.User.Notifications.Settings.Remove(notificationSettings);
                    }
                }
            }

            return changed;
        }
    }
}
