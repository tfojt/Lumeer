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

        // TODOT pass optional Table argument, if specific table is filtered?
        public NewTaskPage()
        {
            InitializeComponent();

            NewTaskViewModel = new NewTaskViewModel(taskTableView.TableSection);
            BindingContext = NewTaskViewModel;
        }

        /*private void TaskTableAttributeWrappersCreated(List<TaskTableAttributeWrapper> taskTableAttributeWrappers)
        {
            taskTableView.TableSection.Clear();

            foreach (var taskTableAttributeWrapper in taskTableAttributeWrappers)
            {
                taskTableView.TableSection.Add(taskTableAttributeWrapper.Cell);
            }
        }*/

        protected override bool OnBackButtonPressed()
        {
            var newTasksViewModel = (NewTaskViewModel)BindingContext;
            MainThread.InvokeOnMainThreadAsync(newTasksViewModel.CheckCancellation);
            return true;
        }
    }
}