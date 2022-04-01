using Lumeer.Models;
using Lumeer.Models.Rest;
using Lumeer.Services;
using Lumeer.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Lumeer.ViewModels
{
    public class TaskDetailViewModel
    {
        public delegate void TaskChangesSavedEventHandler(Task task);
        public event TaskChangesSavedEventHandler TaskChangesSaved;

        private Task _task;
        private Table _table;

        private readonly IAlertService _alertService;
        private readonly INavigationService _navigationService;

        public ICommand SaveCmd { get; set; }

        public List<TaskTableAttributeWrapper> TaskTableAttributeWrappers { get; } = new List<TaskTableAttributeWrapper>();

        public TaskDetailViewModel(Task task, Table table)
        {
            _task = task;
            _table = table;

            _alertService = DependencyService.Get<IAlertService>();
            _navigationService = DependencyService.Get<INavigationService>();

            SaveCmd = new Command(Save);
        }

        private async void Save()
        {
            var changedAttributes = new Dictionary<string, object>();

            foreach (var wrapper in TaskTableAttributeWrappers)
            {
                if (wrapper.ValueChanged(out object newValue))
                {
                    changedAttributes.Add(wrapper.TableAttribute.Id, newValue);
                }
            }

            if (changedAttributes.Count > 0)
            {
                var uri = new Uri(ApiClient.Instance.BaseAddress, $"organizations/{Session.Instance.OrganizationId}/projects/{Session.Instance.ProjectId}/collections/{_table.Id}/documents/{_task.Id}/data");
                string json = JsonConvert.SerializeObject(changedAttributes);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                var method = new HttpMethod("PATCH");
                var request = new HttpRequestMessage(method, uri)
                {
                    Content = stringContent
                };

                HttpResponseMessage response = await ApiClient.Instance.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    await _alertService.DisplayAlert("Error", "Sorry, there was an error while saving data.", "Ok");
                    return;
                }

                foreach (KeyValuePair<string, object> changedAttribute in changedAttributes)
                {
                    if (_task.Data.ContainsKey(changedAttribute.Key))
                    {
                        _task.Data[changedAttribute.Key] = changedAttribute.Value;
                    }
                    else
                    {
                        _task.Data.Add(changedAttribute.Key, changedAttribute.Value);
                    }
                }

                TaskChangesSaved?.Invoke(_task);

                string content = await response.Content.ReadAsStringAsync();
                //var tasks = JsonConvert.DeserializeObject<Tasks>(content);
            }

            await _navigationService.PopAsync();
        }
    }
}
