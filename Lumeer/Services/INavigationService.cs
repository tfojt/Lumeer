using System.Threading.Tasks;
using Xamarin.Forms;

namespace Lumeer.Services
{
    public interface INavigationService
    {
        Task PushAsync(Page page);
        Task PushModalAsync(Page page);
        Task<Page> PopAsync();
        Task<Page> PopModalAsync();
    }
}
