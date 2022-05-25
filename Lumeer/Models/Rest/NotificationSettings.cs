using Lumeer.Models.Rest.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lumeer.Models.Rest
{
    public class NotificationSettings
    {
        public NotificationType NotificationType { get; set; }
        public NotificationChannel NotificationChannel { get; set; }
        public NotificationFrequency NotificationFrequency { get; set; }
    }
}
