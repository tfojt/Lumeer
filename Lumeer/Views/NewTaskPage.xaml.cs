using Lumeer.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Lumeer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewTaskPage : ContentPage
    {
        public NewTaskPage()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            var newTasksViewModel = (NewTaskViewModel)BindingContext;
            MainThread.InvokeOnMainThreadAsync(newTasksViewModel.CheckCancellation);
            return true;
        }
    }
}