using Lumeer.Models.Rest;
using Lumeer.Utils;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace Lumeer.ViewModels
{
    public class TaskDetailViewModel : AbstractTaskViewModel
    {
        public event TaskEventHandler TaskChangesSaved;

        public ICommand SaveCmd { get; set; }

        public TaskDetailViewModel(Task task, Table table, TableSection tableSection)
            : base(task, table, tableSection)
        {
            SaveCmd = new Command(Save);
        }

        private async void Save()
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

            await NavigationService.PopAsync();
        }
    }
}
