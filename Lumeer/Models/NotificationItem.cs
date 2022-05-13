using Lumeer.Models.Rest;
using Lumeer.Models.Rest.Enums;
using Lumeer.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Lumeer.Models
{
    public class NotificationItem : NotifyPropertyChanged
    {
        public Notification Notification { get; set; }

        public string Title { get; set; }

        public Color IconColor { get; set; }

        private string _readContextMenuText;
        public string ReadContextMenuText 
        {
            get => _readContextMenuText;
            set => SetValue(ref _readContextMenuText, value);
        }

        public string DateText { get; set; }

        public List<string> Path { get; set; } = new List<string>() { "somar", "ej"};

        public NotificationItem(Notification notification)
        {
            Notification = notification;

            SetReadContextMenuText();

            ParseNotificationType();

            DateText = new DateTime(notification.CreatedAt).ToString(); // TODOT hours looks correct, date is 1/2/0001
        }

        public void ChangeReadStatus()
        {
            Notification.Read = !Notification.Read;

            SetReadContextMenuText();
        }

        private void ParseNotificationType()
        {
            var notificationType = Enum.Parse(typeof(NotificationType), Notification.Type);
            switch (notificationType)
            {
                case NotificationType.ORGANIZATION_SHARED:
                    {
                        Title = "A new organization has been shared with you.";
                        break;
                    }
                case NotificationType.PROJECT_SHARED:
                    {
                        Title = "A new project has been shared with you.";
                        break;
                    }
                case NotificationType.COLLECTION_SHARED:
                    {
                        Title = "A new table has been shared with you.";
                        break;
                    }
                case NotificationType.VIEW_SHARED:
                    {
                        Title = "A new view has been shared with you.";
                        break;
                    }
                case NotificationType.DUE_DATE_SOON:
                    {
                        Title = "Due Date soon.";
                        break;
                    }
                case NotificationType.DUE_DATE_CHANGED:
                    {
                        Title = "Due Date changed.";
                        break;
                    }
                case NotificationType.PAST_DUE_DATE:
                    {
                        Title = "Past Due Date.";
                        break;
                    }
                case NotificationType.STATE_UPDATE:
                    {
                        Title = "State has been updated.";
                        break;
                    }
                case NotificationType.BULK_ACTION:
                    {
                        Title = "Bulk action.";
                        break;
                    }
                case NotificationType.TASK_ASSIGNED:
                    {
                        Title = GetTaskTitleCommonPart() + "been assigned";
                        break;
                    }
                case NotificationType.TASK_UPDATED:
                    {
                        Title = GetTaskTitleCommonPart() + "been updated";
                        break;
                    }
                case NotificationType.TASK_REMOVED:
                    {
                        Title = GetTaskTitleCommonPart() + "been removed";
                        break;
                    }
                case NotificationType.TASK_UNASSIGNED:
                    {
                        Title = GetTaskTitleCommonPart() + "been unassigned";
                        break;
                    }
                case NotificationType.TASK_COMMENTED:
                    {
                        Title = GetTaskTitleCommonPart() + "new comment.";
                        break;
                    }
                case NotificationType.TASK_MENTIONED:
                    {
                        Title = GetTaskTitleCommonPart() + "new mention.";
                        break;
                    }
                case NotificationType.TASK_REOPENED:
                    {
                        Title = GetTaskTitleCommonPart() + "been reopened.";
                        break;
                    }
                case NotificationType.TASK_CHANGED:
                    {
                        Title = GetTaskTitleCommonPart() + "been changed.";
                        break;
                    }
                default:
                    break;
            }
        }

        private string GetTaskName()
        {
            return (string)Notification.Data["taskName"];
        }

        private string GetTaskTitleCommonPart()
        {
            var taskName = GetTaskName();
            return $"Task {taskName} has ";
        }

        private void SetReadContextMenuText()
        {
            string prefix = Notification.Read ? "un" : "";

            ReadContextMenuText = $"Mark as {prefix}read";
        }
    }
}
