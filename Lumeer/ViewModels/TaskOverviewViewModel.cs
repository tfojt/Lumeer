using Lumeer.Services;
using Lumeer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Lumeer.ViewModels
{
    public class TaskOverviewViewModel : BaseTaskViewModel
    {
        public delegate void TaskDeletedEventHandler(Models.Rest.Task task);
        public event TaskDeletedEventHandler TaskDeleted;

        private readonly IAlertService _alertService;
        private readonly INavigationService _navigationService;

        private const string CANCEL_OPTION = "Cancel";
        private const string DELETE_OPTION = "Delete";
        private const string COPY_LINK_OPTION = "Copy link";

        private Models.Rest.Task _task;

        public IAsyncCommand DisplayOptionsCmd => new AsyncCommand(DisplayOptions);

        public TaskOverviewViewModel(Models.Rest.Task task) : base()
        {
            _task = task;

            _alertService = DependencyService.Get<IAlertService>();
            _navigationService = DependencyService.Get<INavigationService>();
        }

        private async Task DisplayOptions()
        {
            string FAVORITE_OPTION = _task.FavoriteText;

            string option = await _alertService.DisplayActionSheet(null, CANCEL_OPTION, DELETE_OPTION, COPY_LINK_OPTION, FAVORITE_OPTION);
            if (option == null || option == CANCEL_OPTION)
            {
                return;
            }

            if (option == COPY_LINK_OPTION)
            {
                await CopyLink();
                return;
            }

            if (option == FAVORITE_OPTION)
            {
                await ChangeTaskFavoriteStatus(_task);
                return;
            }

            if (option == DELETE_OPTION)
            {
                await DeleteTask();
                return;
            }
        }

        private async Task CopyLink()
        {
            try
            {
                var link = $"https://get.lumeer.io/en/w/{Session.Instance.CurrentOrganization.Code}/{Session.Instance.CurrentProject.Code}/document/{_task.CollectionId}/{_task.Id}";
                await Clipboard.SetTextAsync(link);
            }
            catch (Exception ex)
            {
                await _alertService.DisplayAlert("Error", "Sorry, there was an error while copying link", "Ok", ex);
                return;
            }
        }

        private async Task DeleteTask()
        {
            bool delete = await _alertService.DisplayAlert("Warning", "Do you really want to delete this task?", "Yes", "No");
            if (!delete)
            {
                return;
            }

            try
            {
                await ApiClient.Instance.DeleteTask(_task);
                TaskDeleted?.Invoke(_task);
                await _navigationService.PopAsync();
            }
            catch (Exception ex)
            {
                await _alertService.DisplayAlert("Error", "Sorry, there was an error while deleting task", "Ok", ex);
                return;
            }
        }
    }
}
