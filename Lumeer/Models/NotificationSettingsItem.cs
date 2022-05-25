using Lumeer.Models.Rest;
using Lumeer.Models.Rest.Enums;
using Lumeer.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lumeer.Models
{
    public class NotificationSettingsItem : NotifyPropertyChanged
    {
        public string Type { get; set; }

        public string Hint { get; set; }

        private bool _isToggled;
        public bool IsToggled 
        {
            get => _isToggled;
            set => SetValue(ref _isToggled, value);
        }

        public IEnumerable<NotificationType> NotificationTypes { get; set; }

        public NotificationSettingsItem(string type, string hint, params NotificationType[] notificationTypes)
        {
            Type = type;
            Hint = hint;
            NotificationTypes = notificationTypes;
        }
    }
}
