using Lumeer.Utils;
using System;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
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
        void ShowPopup(BasePopup basePopup);
        Task<T> ShowPopupAsync<T>(Popup<T> popup);
    }
}
