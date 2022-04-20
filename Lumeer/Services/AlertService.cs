using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static Xamarin.Forms.Application;

namespace Lumeer.Services
{
    public class AlertService : IAlertService
    {
        public async Task DisplayAlert(string title, string message, string cancel, Exception debugException = null)
        {
            WriteDebug(title, message, debugException);
            await Current.MainPage.DisplayAlert(title, message, cancel);
        }

        public async Task<bool> DisplayAlert(string title, string message, string accept, string cancel, Exception debugException = null)
        {
            WriteDebug(title, message, debugException);
            return await Current.MainPage.DisplayAlert(title, message, accept, cancel);
        }

        private void WriteDebug(string title, string message, Exception debugException = null)
        {
            string exceptionText = debugException == null ? "" : $": {debugException}";

            Debug.WriteLine($"{title} | {message}{exceptionText}");
        }
    }
}
