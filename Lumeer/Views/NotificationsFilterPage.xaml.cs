using Lumeer.Models;
using Lumeer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Lumeer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotificationsFilterPage : ContentPage
    {
        public NotificationsFilterViewModel NotificationsFilterViewModel { get; set; }

        public NotificationsFilterPage(NotificationsFilterSettings notificationsFilterSettings)
        {
            InitializeComponent();
            NotificationsFilterViewModel = new NotificationsFilterViewModel(notificationsFilterSettings);
            BindingContext = NotificationsFilterViewModel;
        }

        protected override void OnDisappearing()
        {
            NotificationsFilterViewModel.CheckNotificationsFilterSettingsChanged();
        }
    }
}