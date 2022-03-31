using Lumeer.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Lumeer.ViewModels
{
    public class NewTaskViewModel
    {
        private readonly IAlertService _alertService;
        private readonly INavigationService _navigationService;

        public ICommand CancelCmd { get; set; }
        public ICommand CreateCmd { get; set; }

        public NewTaskViewModel()
        {
            _alertService = DependencyService.Get<IAlertService>();
            _navigationService = DependencyService.Get<INavigationService>();
            CancelCmd = new Command(Cancel);
            CreateCmd = new Command(Create);
        }

        public async Task<bool> CheckCancellation()
        {
            bool discardChanges = await _alertService.DisplayAlert("Warning", "Your changes will be discarded!", "Ok", "Cancel");
            if (discardChanges)
            {
                await _navigationService.PopModalAsync();
            }

            return discardChanges;
        }

        private async void Cancel()
        {
            await CheckCancellation();
        }

        private void Create()
        {

        }
    }
}
