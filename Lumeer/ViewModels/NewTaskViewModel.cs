using Lumeer.Models.Rest;
using Lumeer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Task = System.Threading.Tasks.Task;

namespace Lumeer.ViewModels
{
    public class NewTaskViewModel : AbstractTaskViewModel
    {
        public event TaskEventHandler TaskCreated;

        public IAsyncCommand CancelCmd => new AsyncCommand(Cancel);
        public IAsyncCommand CreateCmd => new AsyncCommand(Create);

        public List<Table> Tables { get; } = Session.Instance.TaskTables;

        // TODOT what if user does not have task table?
        public NewTaskViewModel(TableSection tableSection) : base(new Models.Rest.Task(), Session.Instance.TaskTables.First(), tableSection)
        {
        }

        public async Task<bool> CheckCancellation()
        {
            bool discardChanges = await AlertService.DisplayAlert("Warning", "Your changes will be discarded!", "Ok", "Cancel");
            if (discardChanges)
            {
                await NavigationService.PopModalAsync();
            }

            return discardChanges;
        }

        private async Task Cancel()
        {
            await CheckCancellation();
        }

        private async Task Create()
        {
            if (TaskAttributesChanged(out Dictionary<string, object> changedAttributes))
            {
                var newTask = new NewTask(Table.Id, changedAttributes);

                try
                {
                    Task = await ApiClient.Instance.CreateTask(newTask);
                }
                catch (Exception ex)
                {
                    await AlertService.DisplayAlert("Error", "Sorry, there was an error while creating task.", "Ok", ex);
                    return;
                }

                TaskCreated?.Invoke(Task);
            }

            await NavigationService.PopModalAsync();
        }
    }
}
