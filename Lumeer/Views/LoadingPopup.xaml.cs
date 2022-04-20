using System;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms.Xaml;

namespace Lumeer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingPopup : Popup, IDisposable
    {
        public LoadingPopup()
        {
            InitializeComponent();
            Navigation.ShowPopup(this);
        }

        public static async Task<T> DoTask<T>(Func<T> task)
        {
            using (new LoadingPopup())
            {
                return await Task.Run(task);
            }
        }

        public static async Task DoTask(Action task)
        {
            using (new LoadingPopup())
            {
                await Task.Run(task);
            }
        }

        public void Dispose()
        {
            Dismiss(null);
        }
    }
}