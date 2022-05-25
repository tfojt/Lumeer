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
    public partial class NotificationsSettingsPage : ContentPage
    {
        public NotificationsSettingsViewModel NotificationsSettingsViewModel { get; }

        public NotificationsSettingsPage()
        {
            InitializeComponent();
            NotificationsSettingsViewModel = new NotificationsSettingsViewModel();
            BindingContext = NotificationsSettingsViewModel;
        }

        protected override void OnDisappearing()
        {
            NotificationsSettingsViewModel.CheckSettingsChanged();
        }
    }
}