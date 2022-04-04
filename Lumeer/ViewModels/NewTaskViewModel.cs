using Lumeer.Models;
using Lumeer.Models.Rest;
using Lumeer.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Task = Lumeer.Models.Rest.Task;

namespace Lumeer.ViewModels
{
    public class NewTaskViewModel : AbstractTaskViewModel
    {
        public event TaskEventHandler TaskCreated;

        public ICommand CancelCmd { get; set; }
        public ICommand CreateCmd { get; set; }

        public List<Table> Tables { get; } = Session.Instance.TaskTables;

        // TODOT what if user does not have task table?
        public NewTaskViewModel(TableSection tableSection) : base(new Task(), Session.Instance.TaskTables.First(), tableSection)
        {
            //Task = new Task();
            CancelCmd = new Command(Cancel);
            CreateCmd = new Command(Create);
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

        private async void Cancel()
        {
            await CheckCancellation();
        }

        private async void Create()
        {
            if (TaskAttributesChanged(out Dictionary<string, object> changedAttributes))
            {
                var newTask = new NewTask(Table.Id, changedAttributes);
                HttpResponseMessage response = await SendTaskRequest(HttpMethod.Post, newTask);
                if (!response.IsSuccessStatusCode)
                {
                    await AlertService.DisplayAlert("Error", "Sorry, there was an error while creating data.", "Ok");
                    return;
                }

                string content = await response.Content.ReadAsStringAsync();
                Task = JsonConvert.DeserializeObject<Task>(content);

                TaskCreated?.Invoke(Task);
            }

            await NavigationService.PopModalAsync();
        }
    }
}
