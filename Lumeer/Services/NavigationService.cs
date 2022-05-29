using Lumeer.Utils;
using System;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace Lumeer.Services
{
    public class NavigationService : INavigationService
    {
        public event EventHandler<PagePoppedEventArgs> PagePopped;

        public async Task<Page> PopAsync()
        {
            var page = await App.Current.MainPage.Navigation.PopAsync();
            InvokePagePopped(page);
            return page;
        }

        public async Task<Page> PopModalAsync()
        {
            var page = await App.Current.MainPage.Navigation.PopModalAsync();
            InvokePagePopped(page);
            return page;
        }

        private void InvokePagePopped(Page page)
        {
            PagePopped?.Invoke(this, new PagePoppedEventArgs(page));
        }

        public async Task PushAsync(Page page)
        {
            await App.Current.MainPage.Navigation.PushAsync(page);
        }

        public async Task PushModalAsync(Page page)
        {
            await App.Current.MainPage.Navigation.PushModalAsync(page);
        }

        public void ShowPopup(BasePopup basePopup)
        {
            App.Current.MainPage.Navigation.ShowPopup(basePopup);
        }

        public async Task<T> ShowPopupAsync<T>(Popup<T> popup)
        {
            return await App.Current.MainPage.Navigation.ShowPopupAsync(popup);
        }
    }
}
