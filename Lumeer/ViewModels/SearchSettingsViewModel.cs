using Lumeer.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Lumeer.ViewModels
{
    public class SearchSettingsViewModel
    {
        private readonly INavigationService _navigationService;

        public ICommand OkCmd { get; set; }

        public SearchSettingsViewModel()
        {
            _navigationService = DependencyService.Get<INavigationService>();
            OkCmd = new Command(Ok);
        }

        private void Ok()
        {
            _navigationService.PopModalAsync();
        }
    }
}
