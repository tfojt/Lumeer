using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Lumeer.Services
{
    public class NavigationService : INavigationService
    {
        public async Task<Page> PopAsync()
        {
            return await App.Current.MainPage.Navigation.PopAsync();
        }

        public async Task<Page> PopModalAsync()
        {
            return await App.Current.MainPage.Navigation.PopModalAsync();
        }

        public async Task PushAsync(Page page)
        {
            await App.Current.MainPage.Navigation.PushAsync(page);
        }

        public async Task PushModalAsync(Page page)
        {
            await App.Current.MainPage.Navigation.PushModalAsync(page);
        }
    }
}
