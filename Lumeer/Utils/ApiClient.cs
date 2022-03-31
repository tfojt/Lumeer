using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Lumeer.Utils
{
    public sealed class ApiClient
    {
        public static HttpClient Instance { get; private set; }

        public static void SetupInstance()
        {
            Instance = new HttpClient
            {
                BaseAddress = new Uri("http://10.0.2.2:8080/lumeer-engine/rest/")
            };

            //var identityToken = await SecureStorage.GetAsync("identityToken");
            var identityToken = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6Ik16VTRNMFEwTkVVNVJUZ3lRa0ZFUlVZMFJEVkdNREk1UlRVeFFVTkJSRFV4TWprMU16azJSQSJ9.eyJpc3MiOiJodHRwczovL2x1bWVlci5ldS5hdXRoMC5jb20vIiwic3ViIjoiYXV0aDB8NjIwZjk1ZGUzZThhMmIwMDY5ZmUwMTA0IiwiYXVkIjpbImh0dHA6Ly9sb2NhbGhvc3Q6ODA4MC8iLCJodHRwczovL2x1bWVlci5ldS5hdXRoMC5jb20vdXNlcmluZm8iXSwiaWF0IjoxNjQ4NzI4OTE0LCJleHAiOjE2NDg3MzI1MTQsImF6cCI6IkhqZWUwTGEyRGpsWWpJSDVDbEN4M1huZmFqMDJuMk9uIiwic2NvcGUiOiJvcGVuaWQgcHJvZmlsZSBlbWFpbCJ9.Udqr067a6nFwiGu7YTKTl5hlWU_nZqRPOVWuxaKB1oKZxcvcRtZvieaVaGLCm4-giTKnGGOPFzuH9i2dv7h_LUJuS_BL-DQXQuIwRgnW9ghJBcMotwHWTSsBWjUaObffZG-3ftkOCaMmiKMPxedH3LuwjQD59tu6l0mDpr1wpckz6-YMKuKsLNKq17VI94fnrI0HsGeEBs5F6_OE_NKCmiTfFytTCm8nvcks-7oho0bxroLNLjazuxUnLp4dnHx8zNoF2JzfhZCPBzbD7wh6T6pQ4OoErBmVX2Zr6p1cjeyaNy10iayfXhgEns8DXH5krRzJsCepNdVnAxcmbSZYvQ";
            Instance.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", identityToken);
            Instance.DefaultRequestHeaders.Host = "localhost";
        }
    }
}
