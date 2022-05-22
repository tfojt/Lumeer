using Lumeer.Models.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Lumeer.Utils
{
    public sealed class ApiClient
    {
        public static ApiClient Instance { get; } = new ApiClient();

        private static HttpClient _httpClient;

        static ApiClient()
        {
        }

        private ApiClient()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://10.0.2.2:8080/lumeer-engine/rest/")
            };

            _httpClient.DefaultRequestHeaders.Host = "localhost";
        }

        public void Authorize(string accessToken)
        {
            // TODOT what if accessToken expires?
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        private async Task<HttpResponseMessage> SendRequest(HttpMethod method, string uriPart, object payload = null)
        {
            var uri = new Uri(_httpClient.BaseAddress, uriPart);
            var request = new HttpRequestMessage(method, uri);

            if (payload != null)
            {
                var settings = new JsonSerializerSettings 
                { 
                    ContractResolver = new CamelCasePropertyNamesContractResolver(), 
                    Formatting = Formatting.Indented 
                };
                string json = JsonConvert.SerializeObject(payload, settings);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                request.Content = stringContent;
            }

            return await _httpClient.SendAsync(request);
        }

        private async Task<HttpResponseMessage> SendRequestAndEnsureSuccessStatusCode(HttpMethod method, string uriPart, object payload = null)
        {
            var response = await SendRequest(method, uriPart, payload);
            response.EnsureSuccessStatusCode();
            return response;
        }

        private async Task<T> SendRequestGetContent<T>(HttpMethod method, string uriPart, object payload = null)
        {
            var response = await SendRequestAndEnsureSuccessStatusCode(method, uriPart, payload);
            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }

        public async Task<bool> IsAccessTokenValid()
        {
            string uri = "users/current";
            var response = await SendRequest(HttpMethod.Get, uri);
            return response.IsSuccessStatusCode;
        }

        public async Task<User> GetUser()
        {
            string uri = "users/current";
            return await SendRequestGetContent<User>(HttpMethod.Get, uri);
        }

        public async Task<List<Table>> GetTables()
        {
            string uri = $"organizations/{Session.Instance.OrganizationId}/projects/{Session.Instance.ProjectId}/collections";
            return await SendRequestGetContent<List<Table>>(HttpMethod.Get, uri);
        }

        public async Task<List<User>> GetUsers()
        {
            string uri = $"users/organizations/{Session.Instance.OrganizationId}/users";
            return await SendRequestGetContent<List<User>>(HttpMethod.Get, uri);
        }

        public async Task<List<Organization>> GetOrganizations()
        {
            string uri = "organizations";
            return await SendRequestGetContent<List<Organization>>(HttpMethod.Get, uri);
        }

        public async Task<List<SelectionList>> GetSelectionLists()
        {
            string uri = $"organizations/{Session.Instance.OrganizationId}/selection-lists";
            return await SendRequestGetContent<List<SelectionList>>(HttpMethod.Get, uri);
        }

        public async Task<List<Notification>> GetNotifications()
        {
            string uri = $"notifications";
            return await SendRequestGetContent<List<Notification>>(HttpMethod.Get, uri);
        }
        
        public async Task<List<TaskComment>> GetComments(Models.Rest.Task task, int pageStart = 0, int pageLength = 0)
        {
            string uri = $"organizations/{Session.Instance.OrganizationId}/projects/{Session.Instance.ProjectId}/comments/document/{task.Id}?pageStart={pageStart}&pageLength={pageLength}";
            return await SendRequestGetContent<List<TaskComment>>(HttpMethod.Get, uri);
        }

        public async Task<List<TaskActivity>> GetTaskActivity(Models.Rest.Task task)
        {
            string uri = $"organizations/{Session.Instance.OrganizationId}/projects/{Session.Instance.ProjectId}/collections/{task.CollectionId}/documents/{task.Id}/audit";
            return await SendRequestGetContent<List<TaskActivity>>(HttpMethod.Get, uri);
        }
        
        public async Task<List<Models.Rest.Task>> GetActualTasks(params string[] taskIds)
        {
            string uri = $"organizations/{Session.Instance.OrganizationId}/projects/{Session.Instance.ProjectId}/data/documents";
            return await SendRequestGetContent<List<Models.Rest.Task>>(HttpMethod.Post, uri, taskIds);
        }

        public async Task<Tasks> GetTasks(SearchFilter searchFilter)
        {
            string uri = $"organizations/{Session.Instance.OrganizationId}/projects/{Session.Instance.ProjectId}/search/tasks?subItems=false";
            return await SendRequestGetContent<Tasks>(HttpMethod.Post, uri, searchFilter);
        }

        public async Task<Models.Rest.Task> CreateTask(NewTask newTask)
        {
            string uri = $"organizations/{Session.Instance.OrganizationId}/projects/{Session.Instance.ProjectId}/collections/{newTask.CollectionId}/documents";
            return await SendRequestGetContent<Models.Rest.Task>(HttpMethod.Post, uri, newTask);
        }
        
        public async Task<TaskComment> SendComment(TaskComment taskComment)
        {
            string uri = $"organizations/{Session.Instance.OrganizationId}/projects/{Session.Instance.ProjectId}/collections/{taskComment.ParentId}/documents";
            return await SendRequestGetContent<TaskComment>(HttpMethod.Post, uri, taskComment);
        }
        
        public async Task<TaskComment> EditComment(EditedTaskComment editedTaskComment)
        {
            string uri = $"organizations/{Session.Instance.OrganizationId}/projects/{Session.Instance.ProjectId}/comments/document/{editedTaskComment.ResourceId}";
            return await SendRequestGetContent<TaskComment>(HttpMethod.Put, uri, editedTaskComment);
        }
        
        public async Task<HttpResponseMessage> DeleteComment(TaskComment taskComment)
        {
            string uri = $"organizations/{Session.Instance.OrganizationId}/projects/{Session.Instance.ProjectId}/comments/document/{taskComment.ResourceId}/{taskComment.Id}";
            return await SendRequestAndEnsureSuccessStatusCode(HttpMethod.Delete, uri, new object());
        }

        public async Task<Models.Rest.Task> UpdateTask(Models.Rest.Task oldTask, Dictionary<string, object> changedAttributes)
        {
            string uri = $"organizations/{Session.Instance.OrganizationId}/projects/{Session.Instance.ProjectId}/collections/{oldTask.CollectionId}/documents/{oldTask.Id}/data";
            return await SendRequestGetContent<Models.Rest.Task>(new HttpMethod("PATCH"), uri, changedAttributes);
        }

        public async Task<HttpResponseMessage> ChangeTaskFavoriteStatus(Models.Rest.Task task, bool makeFavorite)
        {
            string uri = $"organizations/{Session.Instance.OrganizationId}/projects/{Session.Instance.ProjectId}/collections/{task.CollectionId}/documents/{task.Id}/favorite";
            return await SendRequestAndEnsureSuccessStatusCode(makeFavorite ? HttpMethod.Post : HttpMethod.Delete, uri, new object());
        }

        public async Task<HttpResponseMessage> ChangeNotificationReadStatus(Notification notification, bool read)
        {
            string uri = $"notifications/{notification.Id}";

            var payload = new 
            {
                id = notification.Id,
                userId = notification.UserId,
                read = read
            };
            
            return await SendRequestAndEnsureSuccessStatusCode(HttpMethod.Put, uri, payload);
        }

        public async Task<HttpResponseMessage> DeleteNotification(Notification notification)
        {
            string uri = $"notifications/{notification.Id}";

            var payload = new object();
            
            return await SendRequestAndEnsureSuccessStatusCode(HttpMethod.Delete, uri, payload);
        }
    }
}
