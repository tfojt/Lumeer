using System.Threading.Tasks;

namespace Lumeer.Services
{
    public interface ISecureStorageService
    {
        Task<string> GetAsync(string key);
        Task SetAsync(string key, string value);
        bool Remove(string key);
    }
}
