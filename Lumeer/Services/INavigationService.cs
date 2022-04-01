using Lumeer.Utils;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Lumeer.Services
{
    public interface INavigationService
    {
        event EventHandler<PagePoppedEventArgs> PagePopped;
        Task PushAsync(Page page);
        Task PushModalAsync(Page page);
        Task<Page> PopAsync();
        Task<Page> PopModalAsync();
    }
}
