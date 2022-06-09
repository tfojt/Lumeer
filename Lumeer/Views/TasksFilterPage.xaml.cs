using Lumeer.Models;
using Lumeer.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Lumeer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TasksFilterPage : ContentPage
    {
        public TasksFilterViewModel TasksFilterViewModel { get; set; }

        public TasksFilterPage(TasksFilterSettings tasksFilterSettings)
        {
            InitializeComponent();
            TasksFilterViewModel = new TasksFilterViewModel(tasksFilterSettings);
            BindingContext = TasksFilterViewModel;
        }

        protected async override void OnDisappearing()
        {
            await TasksFilterViewModel.CheckTasksFilterSettingsChanged();
        }
    }
}