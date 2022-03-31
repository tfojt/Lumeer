using System.Threading.Tasks;
using static Xamarin.Forms.Application;

namespace Lumeer.Services
{
    public class AlertService : IAlertService
    {
        public async Task DisplayAlert(string title, string message, string cancel)
        {
            await Current.MainPage.DisplayAlert(title, message, cancel);
        }

        public async Task<bool> DisplayAlert(string title, string message, string accept, string cancel)
        {
            return await Current.MainPage.DisplayAlert(title, message, accept, cancel);
        }
    }
}
