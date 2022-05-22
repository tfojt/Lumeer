using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Lumeer.Services
{
    public class SecureStorageService : ISecureStorageService
    {
        public const string ACCESS_TOKEN_KEY = "accessToken";

        public async Task<string> GetAsync(string key)
        {
            try
            {
                return await SecureStorage.GetAsync(key);
            }
            catch (Exception ex)
            {
                ProcessException(ex);
                return null;
            }
        }

        public async Task SetAsync(string key, string value)
        {
            try
            {
                await SecureStorage.SetAsync(key, value);
            }
            catch (Exception ex)
            {
                ProcessException(ex);
            }
        }

        public bool Remove(string key)
        {
            try
            {
                return SecureStorage.Remove(key);
            }
            catch (Exception ex)
            {
                ProcessException(ex);
                return false;
            }
        }

        private void ProcessException(Exception ex)
        {
            // Possible that device doesn't support secure storage on device.
            Debug.WriteLine(ex);
        }
    }
}
