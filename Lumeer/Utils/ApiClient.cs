using Lumeer.Models;
using Lumeer.Models.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static Lumeer.Utils.EventHandlers;

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
            string uri = $"organizations/{Session.Instance.CurrentOrganization.Id}/projects/{Session.Instance.CurrentProject.Id}/collections";
            return await SendRequestGetContent<List<Table>>(HttpMethod.Get, uri);
        }

        public async Task<List<User>> GetUsers()
        {
            string uri = $"users/organizations/{Session.Instance.CurrentOrganization.Id}/users";
            return await SendRequestGetContent<List<User>>(HttpMethod.Get, uri);
        }

        public async Task<List<Organization>> GetOrganizations()
        {
            string uri = "organizations";
            return await SendRequestGetContent<List<Organization>>(HttpMethod.Get, uri);
        }
        
        public async Task<List<Project>> GetProjects()
        {
            string uri = $"organizations/{Session.Instance.CurrentOrganization.Id}/projects";
            return await SendRequestGetContent<List<Project>>(HttpMethod.Get, uri);
        }

        public async Task<List<SelectionList>> GetSelectionLists()
        {
            string uri = $"organizations/{Session.Instance.CurrentOrganization.Id}/selection-lists";
            return await SendRequestGetContent<List<SelectionList>>(HttpMethod.Get, uri);
        }

        public async Task<List<Notification>> GetNotifications()
        {
            string uri = $"notifications";
            return await SendRequestGetContent<List<Notification>>(HttpMethod.Get, uri);
        }
        
        public async Task<List<TaskComment>> GetComments(Models.Rest.Task task, int pageStart = 0, int pageLength = 0)
        {
            string uri = $"organizations/{Session.Instance.CurrentOrganization.Id}/projects/{Session.Instance.CurrentProject.Id}/comments/document/{task.Id}?pageStart={pageStart}&pageLength={pageLength}";
            return await SendRequestGetContent<List<TaskComment>>(HttpMethod.Get, uri);
        }

        public async Task<List<TaskActivity>> GetTaskActivity(Models.Rest.Task task)
        {
            string uri = $"organizations/{Session.Instance.CurrentOrganization.Id}/projects/{Session.Instance.CurrentProject.Id}/collections/{task.CollectionId}/documents/{task.Id}/audit";
            return await SendRequestGetContent<List<TaskActivity>>(HttpMethod.Get, uri);
        }
        
        public async Task<List<LinkType>> GetLinkTypes()
        {
            string uri = $"organizations/{Session.Instance.CurrentOrganization.Id}/projects/{Session.Instance.CurrentProject.Id}/link-types";
            return await SendRequestGetContent<List<LinkType>>(HttpMethod.Get, uri);
        }
        
        public async Task<List<Models.Rest.Task>> GetActualTasks(params string[] taskIds)
        {
            string uri = $"organizations/{Session.Instance.CurrentOrganization.Id}/projects/{Session.Instance.CurrentProject.Id}/data/documents";
            return await SendRequestGetContent<List<Models.Rest.Task>>(HttpMethod.Post, uri, taskIds);
        }

        public async Task<Tasks> GetTasks(TasksFilterSettings tasksFilterSettings)
        {
            string uri = $"organizations/{Session.Instance.CurrentOrganization.Id}/projects/{Session.Instance.CurrentProject.Id}/search/tasks?subItems={tasksFilterSettings.IncludeSubItems}";
            return await SendRequestGetContent<Tasks>(HttpMethod.Post, uri, tasksFilterSettings.TasksFilter);
        }
        
        public async Task<List<Document>> GetDocuments(string collectionId)
        {
            string uri = $"organizations/{Session.Instance.CurrentOrganization.Id}/projects/{Session.Instance.CurrentProject.Id}/search/documents?subItems=false";

            var stems = new List<object>
            {
                new
                {
                    collectionId = collectionId
                }
            };

            var payload = new
            {
                stems = stems
            };
            
            return await SendRequestGetContent<List<Document>>(HttpMethod.Post, uri, payload);
        }
        
        public async Task<List<Link>> GetLinks(string collectionId, string linkTypeId)
        {
            string uri = $"organizations/{Session.Instance.CurrentOrganization.Id}/projects/{Session.Instance.CurrentProject.Id}/search/linkInstances?subItems=false";

            var stems = new List<object>
            {
                new
                {
                    collectionId = collectionId,
                    linkTypeIds = new List<string>
                    {
                        linkTypeId
                    }
                }
            };

            var payload = new
            {
                stems = stems
            };

            return await SendRequestGetContent<List<Link>>(HttpMethod.Post, uri, payload);
        }

        public async Task<List<Link>> EditDocumentLinks(string linkTypeId, string documentId, IEnumerable<string> removedLinkInstancesIds, IEnumerable<NewLink> createdLinkInstances)
        {
            string uri = $"organizations/{Session.Instance.CurrentOrganization.Id}/projects/{Session.Instance.CurrentProject.Id}/link-instances/{linkTypeId}/documentLinks";

            var payload = new
            {
                documentId = documentId,
                removedLinkInstancesIds = removedLinkInstancesIds,
                createdLinkInstances = createdLinkInstances
            };

            return await SendRequestGetContent<List<Link>>(HttpMethod.Post, uri, payload);
        }
        
        public async Task<List<Link>> GetActualLinks(params string[] linkIds)
        {
            string uri = $"organizations/{Session.Instance.CurrentOrganization.Id}/projects/{Session.Instance.CurrentProject.Id}/data/linkInstances";
            return await SendRequestGetContent<List<Link>>(HttpMethod.Post, uri, linkIds);
        }

        public async Task<LinkType> CreateLinkType(NewLinkType newLinkType)
        {
            string uri = $"organizations/{Session.Instance.CurrentOrganization.Id}/projects/{Session.Instance.CurrentProject.Id}/link-types";

            var payload = new
            {
                name = newLinkType.Name,
                collectionIds = new string[] { newLinkType.CurrentTableId, newLinkType.LinkedTableId },
                attributes = new object[0] { },
                rules = new object()
            };

            return await SendRequestGetContent<LinkType>(HttpMethod.Post, uri, payload);
        }

        public async Task<Models.Rest.Task> CreateTask(NewTask newTask)
        {
            string uri = $"organizations/{Session.Instance.CurrentOrganization.Id}/projects/{Session.Instance.CurrentProject.Id}/collections/{newTask.CollectionId}/documents";
            return await SendRequestGetContent<Models.Rest.Task>(HttpMethod.Post, uri, newTask);
        }
        
        public async Task<TaskComment> SendComment(string comment, string taskId)
        {
            string uri = $"organizations/{Session.Instance.CurrentOrganization.Id}/projects/{Session.Instance.CurrentProject.Id}/comments/document/{taskId}";

            var payload = new
            {
                comment = comment,
                resourceId = taskId,
                resourceType = "DOCUMENT"
            };
            
            return await SendRequestGetContent<TaskComment>(HttpMethod.Post, uri, payload);
        }
        
        public async Task<TaskComment> EditComment(EditedTaskComment editedTaskComment)
        {
            string uri = $"organizations/{Session.Instance.CurrentOrganization.Id}/projects/{Session.Instance.CurrentProject.Id}/comments/document/{editedTaskComment.ResourceId}";
            return await SendRequestGetContent<TaskComment>(HttpMethod.Put, uri, editedTaskComment);
        }
        
        public async Task<HttpResponseMessage> DeleteComment(TaskComment taskComment)
        {
            string uri = $"organizations/{Session.Instance.CurrentOrganization.Id}/projects/{Session.Instance.CurrentProject.Id}/comments/document/{taskComment.ResourceId}/{taskComment.Id}";
            return await SendRequestAndEnsureSuccessStatusCode(HttpMethod.Delete, uri, new object());
        }

        public async Task<Models.Rest.Task> UpdateTask(Models.Rest.Task oldTask, Dictionary<string, object> changedAttributes)
        {
            string uri = $"organizations/{Session.Instance.CurrentOrganization.Id}/projects/{Session.Instance.CurrentProject.Id}/collections/{oldTask.CollectionId}/documents/{oldTask.Id}/data";
            return await SendRequestGetContent<Models.Rest.Task>(new HttpMethod("PATCH"), uri, changedAttributes);
        }

        public async Task<HttpResponseMessage> ChangeTaskFavoriteStatus(Models.Rest.Task task, bool makeFavorite)
        {
            string uri = $"organizations/{Session.Instance.CurrentOrganization.Id}/projects/{Session.Instance.CurrentProject.Id}/collections/{task.CollectionId}/documents/{task.Id}/favorite";
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
            return await SendRequestAndEnsureSuccessStatusCode(HttpMethod.Delete, uri, new object());
        }

        public async Task<HttpResponseMessage> DeleteTask(Models.Rest.Task task)
        {
            string uri = $"organizations/{Session.Instance.CurrentOrganization.Id}/projects/{Session.Instance.CurrentProject.Id}/collections/{task.CollectionId}/documents/{task.Id}";
            return await SendRequestAndEnsureSuccessStatusCode(HttpMethod.Delete, uri, new object());
        }

        public async Task<HttpResponseMessage> ChangeWorkspace()
        {
            string uri = $"users/workspace";

            var payload = new
            {
                organizationId = Session.Instance.CurrentOrganization.Id,
                projectId = Session.Instance.CurrentProject.Id
            };

            return await SendRequestAndEnsureSuccessStatusCode(HttpMethod.Put, uri, payload);
        }

        public async Task<HttpResponseMessage> ChangeNotificationsSettings(User user)
        {
            string uri = $"users/current";

            var payload = new
            {
                hints = user.Hints,
                name = user.Name,
                notifications = user.Notifications
            };

            return await SendRequestAndEnsureSuccessStatusCode(new HttpMethod("PATCH"), uri, payload);
        }
    }
}
