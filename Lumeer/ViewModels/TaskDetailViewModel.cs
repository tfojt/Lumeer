using Lumeer.Models;
using Lumeer.Models.Rest;
using System;
using System.Collections.Generic;
using System.Net.Http;
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
                var method = new HttpMethod("PATCH");
                HttpResponseMessage response = await SendTaskRequest(method, changedAttributes);
                if (!response.IsSuccessStatusCode)
                {
                    await AlertService.DisplayAlert("Error", "Sorry, there was an error while saving data.", "Ok");
                    return;
                }

                UpdateTaskDataAttributes(changedAttributes);

                TaskChangesSaved?.Invoke(Task);
            }

            await NavigationService.PopAsync();
        }
    }
}
