using Lumeer.Models;
using Lumeer.Models.Rest;
using Lumeer.ViewModels;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Lumeer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewTaskPage : ContentPage
    {
        public NewTaskViewModel NewTaskViewModel { get; }

        public NewTaskPage()
        {
            InitializeComponent();

            NewTaskViewModel = new NewTaskViewModel(tableSection);
            BindingContext = NewTaskViewModel;
        }

        protected override bool OnBackButtonPressed()
        {
            var newTasksViewModel = (NewTaskViewModel)BindingContext;
            MainThread.InvokeOnMainThreadAsync(newTasksViewModel.CheckCancellation);
            return true;
        }
    }
}