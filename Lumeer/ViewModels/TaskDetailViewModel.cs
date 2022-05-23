using Lumeer.Models.Rest;
using Lumeer.Utils;
using System;
using System.Collections.Generic;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Task = System.Threading.Tasks.Task;

namespace Lumeer.ViewModels
{
    public class TaskDetailViewModel : AbstractTaskViewModel
    {
        public event TaskEventHandler TaskChangesSaved;

        public IAsyncCommand SaveCmd => new AsyncCommand(Save);

        public TaskDetailViewModel(Models.Rest.Task task, Table table, TableSection tableSection)
            : base(task, table, tableSection)
        {
        }

        private async Task Save()
        {
            if (TaskAttributesChanged(out Dictionary<string, object> changedAttributes))
            {
                try
                {
                    Task = await ApiClient.Instance.UpdateTask(Task, changedAttributes);
                }
                catch (Exception ex)
                {
                    await AlertService.DisplayAlert("Error", "Sorry, there was an error while saving task.", "Ok", ex);
                    return;
                }

                TaskChangesSaved?.Invoke(Task);
            }
        }
    }
}
