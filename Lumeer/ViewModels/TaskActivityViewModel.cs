using Lumeer.Models;
using Lumeer.Models.Rest;
using Lumeer.Services;
using Lumeer.Utils;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Task = System.Threading.Tasks.Task;

namespace Lumeer.ViewModels
{
    public class TaskActivityViewModel : BaseViewModel
    {
        public IAsyncCommand LoadActivityCmd => new AsyncCommand(LoadActivity, allowsMultipleExecutions: false);

        public ObservableCollection<TaskActivityItem> Activities { get; set; } = new ObservableCollection<TaskActivityItem>();

        private bool _loadingActivity;
        public bool LoadingActivity
        {
            get => _loadingActivity;
            set => SetValue(ref _loadingActivity, value);
        }

        private Models.Rest.Task _task;
        private Table _table;
        private readonly IAlertService _alertService;

        public TaskActivityViewModel(Models.Rest.Task task, Table table)
        {
            _task = task;
            _table = table;
            _alertService = DependencyService.Get<IAlertService>();

            Task.Run(LoadActivity);
        }

        private async Task LoadActivity()
        {
            LoadingActivity = true;

            try
            {
                var acitvities = await ApiClient.Instance.GetTaskActivity(_task);

                Activities.Clear();

                foreach (var activity in acitvities)
                {
                    var activityItem = new TaskActivityItem(activity, _table.Attributes);
                    Activities.Add(activityItem);
                }

                var taskCreatedActivity = new TaskActivityItem(_task);
                Activities.Add(taskCreatedActivity);
            }
            catch (Exception ex)
            {
                var message = "Could not load activity";
                await _alertService.DisplayAlert("Error", message, "Ok", ex);
            }
            finally
            {
                LoadingActivity = false;
            }
        }
    }
}
