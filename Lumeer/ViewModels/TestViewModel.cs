using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Lumeer.Models.Rest;
using Lumeer.Utils;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Lumeer.ViewModels
{
    public class TestViewModel
    {
        private readonly HttpClient _client = new HttpClient();

        public ICommand TestCmd { get; set; }

        public TestViewModel()
        {
            /*_client = new HttpClient
            {
                BaseAddress = new Uri("http://10.0.2.2:8080/lumeer-engine/rest/"),
            };

            //var accessToken = await SecureStorage.GetAsync("accessToken");
            //var identityToken = await SecureStorage.GetAsync("identityToken");
            var identityToken = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6Ik16VTRNMFEwTkVVNVJUZ3lRa0ZFUlVZMFJEVkdNREk1UlRVeFFVTkJSRFV4TWprMU16azJSQSJ9.eyJpc3MiOiJodHRwczovL2x1bWVlci5ldS5hdXRoMC5jb20vIiwic3ViIjoiYXV0aDB8NjIwZjk1ZGUzZThhMmIwMDY5ZmUwMTA0IiwiYXVkIjpbImh0dHA6Ly9sb2NhbGhvc3Q6ODA4MC8iLCJodHRwczovL2x1bWVlci5ldS5hdXRoMC5jb20vdXNlcmluZm8iXSwiaWF0IjoxNjQ4Mjk5ODUxLCJleHAiOjE2NDgzMDM0NTEsImF6cCI6IkhqZWUwTGEyRGpsWWpJSDVDbEN4M1huZmFqMDJuMk9uIiwic2NvcGUiOiJvcGVuaWQgcHJvZmlsZSBlbWFpbCJ9.hh3Nm4UopeBCVihCiRKQS9RBG56sjGqVnic40X1-IA3RqtkuQfQO5tFVlOEqkowPupgl5o3J2jz892AD5httNCQkGvdZxTj0Q7IrD9aH8dco9Kfw88UpVsK7LUI6LsGD4JIgoYBBNT4TvbrbUk15Bi11nD4Q2Y1z6ljzjcnittud75t0gsidFeaWyg4BcRWrZCrOHmw7JWkBt2Z_5yTv-3TQ3BlYUXK0fzXnsjvWpNnkJsWDqSXV03yiGDb4DxqdYoEtksMJVEeucYgBamzUPFd5REYPCJWDT0Ho4tv-tfgPYcm9Tn1F14jKYtd67dY5KV1X291HhvNzkQq3cJxRXw";
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", identityToken);
            _client.DefaultRequestHeaders.Host = "localhost";
            //AddClientHeaders();*/

            TestCmd = new Command(Test);
        }

        private async void Test()
        {
            var user = await GetUser();
            var tables = await GetTables(user);
            var tasks = await GetTasks(user);
        }

        private async Task<User> GetUser()
        {
            var uri = new Uri(_client.BaseAddress, "users/current");
            HttpResponseMessage response = await _client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<User>(content);
            return user;
        }
        
        private async Task<List<Table>> GetTables(User user)
        {
            var uri = new Uri(_client.BaseAddress, $"organizations/{user.DefaultWorkspace.OrganizationId}/projects/{user.DefaultWorkspace.ProjectId}/collections");
            HttpResponseMessage response = await _client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            var tables = JsonConvert.DeserializeObject<List<Table>>(content);
            return tables;
        }

        private async Task<Tasks> GetTasks(User user)
        {
            var uri = new Uri(_client.BaseAddress, $"organizations/{user.DefaultWorkspace.OrganizationId}/projects/{user.DefaultWorkspace.ProjectId}/search/tasks?subItems=false");
            string json = "{\"stems\":[{\"id\":\"164822630029910cc6d7cf033a\",\"collectionId\":\"623c9626a1985c3b3356f829\",\"documentIds\":[],\"linkTypeIds\":[],\"filters\":[],\"linkFilters\":[]}],\"fulltexts\":[],\"page\":null,\"pageSize\":null}";
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync(uri, stringContent);
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            var tasks = JsonConvert.DeserializeObject<Tasks>(content);
            return tasks;
        }

        private void AddClientHeaders()
        {
            _client.DefaultRequestHeaders.Add("Accept", new string[]{ "application/json" , "text/plain", "*/*" });
            _client.DefaultRequestHeaders.Add("Accept-Encoding", new string[]{ "gzip", "deflate", "br" });
            _client.DefaultRequestHeaders.Add("Accept-Language", new string[]{ "cs-CZ", "cs" });    // "q=0.9" invalid
            _client.DefaultRequestHeaders.Add("Connection", "keep-alive");
        }
    }
}
