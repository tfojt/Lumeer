using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
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

        public static void Authorize()
        {
            //var identityToken = await SecureStorage.GetAsync("identityToken");
            var identityToken = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6Ik16VTRNMFEwTkVVNVJUZ3lRa0ZFUlVZMFJEVkdNREk1UlRVeFFVTkJSRFV4TWprMU16azJSQSJ9.eyJpc3MiOiJodHRwczovL2x1bWVlci5ldS5hdXRoMC5jb20vIiwic3ViIjoiYXV0aDB8NjIwZjk1ZGUzZThhMmIwMDY5ZmUwMTA0IiwiYXVkIjpbImh0dHA6Ly9sb2NhbGhvc3Q6ODA4MC8iLCJodHRwczovL2x1bWVlci5ldS5hdXRoMC5jb20vdXNlcmluZm8iXSwiaWF0IjoxNjQ5MDE0NDIwLCJleHAiOjE2NDkwMTgwMjAsImF6cCI6IkhqZWUwTGEyRGpsWWpJSDVDbEN4M1huZmFqMDJuMk9uIiwic2NvcGUiOiJvcGVuaWQgcHJvZmlsZSBlbWFpbCJ9.ArWSwvXcgybavHqFtEwfwZSG8Ccr3zmkKbU_8cEimaKIFMuFWtAwN4nNDJaogWa4l0v_ZfM7D_pFRrfWyQyk9wzt-vnP_dJpx1AuklXJwR5YXidGZ-iMejYuhC-aTL1CCgb1Uv_F34aNyTkl68f9Z8Fs9RVnLhByTSF9dAgw6ju1MHyGToPNOcKBvxAOy17l_e844D8eDL-XfKtpny6HGSQA0HACkb5HghlRJov0vgyKj-89ilPy2oIvigC6gprZj_aQG_K-nyChMPUcFHegAJpCBu_KfQFDETlaz1M8y1G5WSpwaLwHNRC4TaBcRwSNU6Z5SdOnQoXZTtREab_LOQ";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", identityToken);
        }

        public async Task<HttpResponseMessage> SendRequest(HttpMethod method, string uriPart, object payload = null)
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

        public async Task<T> SendRequestGetContent<T>(HttpMethod method, string uriPart, object payload = null)
        {
            var response = await SendRequest(method, uriPart, payload);
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}
