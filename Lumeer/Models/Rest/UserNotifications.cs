using System;
using System.Collections.Generic;
using System.Text;

namespace Lumeer.Models.Rest
{
    public class UserNotifications
    {
        public List<NotificationSettings> Settings { get; set; }
        public string Language { get; set; }
    }
}
