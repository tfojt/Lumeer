﻿using Lumeer.Fonts;
using Lumeer.Models.Rest;
using Lumeer.Models.Rest.Enums;
using Lumeer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Lumeer.Models
{
    public class NotificationItem : NotifyPropertyChanged
    {
        public Notification Notification { get; set; }

        public string Title { get; set; }

        public string MainIconFontFamily { get; set; }
        public string MainIconGlyph { get; set; }
        public Color MainIconColor { get; set; }

        private string _readContextMenuText;
        public string ReadContextMenuText 
        {
            get => _readContextMenuText;
            set => SetValue(ref _readContextMenuText, value);
        }

        public string DateText { get; set; }

        public List<IconLabel> Path { get; set; } = new List<IconLabel>();

        private Color _titleColor;
        public Color TitleColor 
        {
            get => _titleColor;
            set => SetValue(ref _titleColor, value);
        }

        private string _readIconGlyph;
        public string ReadIconGlyph
        {
            get => _readIconGlyph;
            set => SetValue(ref _readIconGlyph, value);
        }

        public NotificationItem(Notification notification)
        {
            Notification = notification;

            ApplyAccordingToReadStatus();

            ParseNotificationType();

            DateText = DateTimeOffset.FromUnixTimeMilliseconds(notification.CreatedAt).ToString("M/d/yy, h:mm tt");

            MainIconFontFamily = FontAwesomeAliases.PRO_REGULAR;
            MainIconGlyph = FontAwesomeIcons.CircleQuestion;
            MainIconColor = GetMainIconColor();

            GeneratePath();
        }

        private void GeneratePath()
        {
            foreach (string part in new string[] { "organization", "project", "collection" })
            {
                if (Notification.Data.ContainsKey($"{part}Id"))
                {
                    var partColor = (string)Notification.Data[$"{part}Color"];

                    string partName = Notification.Data.TryGetValue($"{part}Code", out object codeObj) ?
                        (string)codeObj :
                        (string)Notification.Data[$"{part}Name"];

                    var partIcon = new FontImageData(FontAwesomeAliases.PRO_REGULAR, FontAwesomeIcons.CircleQuestion, partColor);
                    var iconLabel = new IconLabel(partIcon, partName);
                    Path.Add(iconLabel);
                }
            }
        }

        private Color GetMainIconColor()
        {
            foreach (string part in new string[] { "collection", "project", "organization" })
            {
                if (Notification.Data.TryGetValue($"{part}Color", out object hexColor))
                {
                    return Color.FromHex((string)hexColor);
                }
            }

            return Color.Gray;
        }

        public void ChangeReadStatus()
        {
            Notification.Read = !Notification.Read;

            ApplyAccordingToReadStatus();
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
                        Title = $"The task {GetTaskName()} has due date soon {GetTaskDueDate()}.";
                        break;
                    }
                case NotificationType.DUE_DATE_CHANGED:
                    {
                        Title = $"The task {GetTaskName()} due date has been changed {GetTaskDueDate()}.";
                        break;
                    }
                case NotificationType.PAST_DUE_DATE:
                    {
                        Title = $"The task {GetTaskName()} is past due date {GetTaskDueDate()}.";
                        break;
                    }
                case NotificationType.STATE_UPDATE:
                    {
                        Title = $"The task {GetTaskName()} state has been updated.";
                        break;
                    }
                case NotificationType.BULK_ACTION:
                    {
                        Title = "Bulk action.";
                        break;
                    }
                case NotificationType.TASK_ASSIGNED:
                    {
                        Title = GetTaskTitleCommonPart() + "been assigned.";
                        break;
                    }
                case NotificationType.TASK_UPDATED:
                    {
                        Title = GetTaskTitleCommonPart() + "been updated.";
                        break;
                    }
                case NotificationType.TASK_REMOVED:
                    {
                        Title = GetTaskTitleCommonPart() + "been removed.";
                        break;
                    }
                case NotificationType.TASK_UNASSIGNED:
                    {
                        Title = GetTaskTitleCommonPart() + "been unassigned.";
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

        private string GetTaskDueDate()
        {
            return (string)Notification.Data["taskDueDate"];
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
        
        private void ApplyAccordingToReadStatus()
        {
            if (Notification.Read)
            {
                TitleColor = Color.Gray;
                ReadIconGlyph = FontAwesomeIcons.Eye;
                ReadContextMenuText = "Unread";
            }
            else
            {
                TitleColor = Color.Black;
                ReadIconGlyph = FontAwesomeIcons.EyeSlash;
                ReadContextMenuText = "Read";
            }
        }
    }
}
