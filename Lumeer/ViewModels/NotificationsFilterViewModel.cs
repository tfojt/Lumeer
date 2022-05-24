using Lumeer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using static Lumeer.Utils.EventHandlers;

namespace Lumeer.ViewModels
{
    public class NotificationsFilterViewModel : BaseViewModel
    {
        public event EmptyEventHandler NotificationsFilterChanged;

        private bool _onlyUnread;
        public bool OnlyUnread
        {
            get => _onlyUnread;
            set => SetValue(ref _onlyUnread, value);
        }

        NotificationsFilterSettings _notificationsFilterSettings;

        public NotificationsFilterViewModel(NotificationsFilterSettings notificationsFilterSettings)
        {
            _notificationsFilterSettings = notificationsFilterSettings;

            OnlyUnread = notificationsFilterSettings.OnlyUnread;
        }


        private bool CheckOnlyUnreadChanged()
        {
            if (_notificationsFilterSettings.OnlyUnread == OnlyUnread)
            {
                return false;
            }

            _notificationsFilterSettings.OnlyUnread = OnlyUnread;
            return true;
        }


        public void CheckNotificationsFilterSettingsChanged()
        {
            bool onlyUnreadChanged = CheckOnlyUnreadChanged();

            if (onlyUnreadChanged)
            {
                NotificationsFilterChanged?.Invoke();
            }
        }
    }
}
