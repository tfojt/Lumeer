using Lumeer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
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

        public ICommand ChangeOnlyUnreadCmd => new Command(ChangeOnlyUnread);

        NotificationsFilterSettings _notificationsFilterSettings;

        public NotificationsFilterViewModel(NotificationsFilterSettings notificationsFilterSettings)
        {
            _notificationsFilterSettings = notificationsFilterSettings;

            OnlyUnread = notificationsFilterSettings.OnlyUnread;
        }

        private void ChangeOnlyUnread()
        {
            OnlyUnread = !OnlyUnread;
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
