using Lumeer.Models;
using Lumeer.Models.Rest;
using Lumeer.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Lumeer.ViewModels
{
    public class TaskDetailViewModel
    {
        private Task _task;
        private Table _table;

        public ICommand SaveCmd { get; set; }

        public List<TaskTableAttributeWrapper> TaskTableAttributeWrappers { get; } = new List<TaskTableAttributeWrapper>();

        public TaskDetailViewModel(Task task, Table table)
        {
            _task = task;
            _table = table;

            SaveCmd = new Command(Save);
        }

        private async void Save()
        {
            var changedAttributes = new Dictionary<string, object>();

            foreach (var wrapper in TaskTableAttributeWrappers)
            {
                if (wrapper.ValueChanged(out object newValue))
                {
                    changedAttributes.Add(wrapper.TableAttribute.Name, newValue);
                }
            }

            if (changedAttributes.Count == 0)
            {
                return;
            }

            var uri = new Uri(ApiClient.Instance.BaseAddress, $"organizations/{Session.Instance.OrganizationId}/projects/{Session.Instance.ProjectId}/collections/{_table.Id}/documents/{_task.Id}/data");
            //string json = "{ \"a1\": \"02.04.2022 18:46\", \"a2\": \"Znovu zmeneno - Se psem\" }";
            string json = JsonConvert.SerializeObject(changedAttributes);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, uri)
            {
                Content = stringContent
            };

            HttpResponseMessage response = await ApiClient.Instance.SendAsync(request);

            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            //var tasks = JsonConvert.DeserializeObject<Tasks>(content);
        }
    }
}
