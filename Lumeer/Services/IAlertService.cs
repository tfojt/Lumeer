using System;
using System.Threading.Tasks;

namespace Lumeer.Services
{
    public interface IAlertService
    {
        Task DisplayAlert(string title, string message, string cancel, Exception debugException = null);
        Task<bool> DisplayAlert(string title, string message, string accept, string cancel, Exception debugException = null);
        Task<string> DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons);
    }
}
