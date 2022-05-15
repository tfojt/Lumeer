using Lumeer.Models.Rest;
using Lumeer.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Lumeer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskDetailView : ContentView
    {
        public TaskDetailViewModel TaskDetailViewModel { get; }

        public TaskDetailView(Models.Rest.Task task, Table table)
        {
            InitializeComponent();

            TaskDetailViewModel = new TaskDetailViewModel(task, table, tableSection);
            BindingContext = TaskDetailViewModel;
        }
    }
}