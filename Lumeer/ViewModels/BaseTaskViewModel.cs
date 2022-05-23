using Lumeer.Services;
using Lumeer.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Lumeer.ViewModels
{
    public abstract class BaseTaskViewModel : BaseViewModel
    {
        protected readonly IAlertService AlertService;

        public BaseTaskViewModel()
        {
            AlertService = DependencyService.Get<IAlertService>();
        }

        protected async Task ChangeTaskFavoriteStatus(Models.Rest.Task task)
        {
            try
            {
                bool newFavoriteValue = !task.Favorite;
                await ApiClient.Instance.ChangeTaskFavoriteStatus(task, newFavoriteValue);
                task.Favorite = newFavoriteValue;
            }
            catch (Exception ex)
            {
                await AlertService.DisplayAlert("Error", "Sorry, there was an error while changing favorite status", "Ok", ex);
                return;
            }
        }

    }
}
