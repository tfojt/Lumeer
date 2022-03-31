using Lumeer.Utils;
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
        private Models.Rest.Task _task;
        private Models.Rest.Table _table;

        public ICommand SaveCmd { get; set; }

        public TaskDetailViewModel(Models.Rest.Task task, Models.Rest.Table table)
        {
            _task = task;
            _table = table;

            SaveCmd = new Command(Save);
        }

        private async void Save()
        {
            var uri = new Uri(ApiClient.Instance.BaseAddress, $"organizations/{Session.Instance.OrganizationId}/projects/{Session.Instance.ProjectId}/collections/{_table.Id}/documents/{_task.Id}/data");
            //string json = "{\"stems\":[{\"collectionId\":\"62402061a26fa76666627730\",\"documentIds\":[],\"linkTypeIds\":[],\"filters\":[],\"linkFilters\":[]}],\"fulltexts\":[],\"page\":null,\"pageSize\":null}";
            string json = "{ \"a2\": \"Zmeneno - Se psem\" }";
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
