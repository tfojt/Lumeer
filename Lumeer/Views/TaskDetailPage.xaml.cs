using Lumeer.Models;
using Lumeer.Models.Rest;
using Lumeer.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Lumeer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskDetailPage : ContentPage
    {
        public TaskDetailViewModel TaskDetailViewModel { get; }

        public TaskDetailPage(Task task, Table table)
        {
            TaskDetailViewModel = new TaskDetailViewModel(task, table);
            BindingContext = TaskDetailViewModel;

            InitializeComponent();

            foreach (TableAttribute tableAttribute in table.Attributes)
            {
                var taskTableAttributeWrapper = new TaskTableAttributeWrapper(task, tableAttribute);
                tableSection.Add(taskTableAttributeWrapper.Cell);
                TaskDetailViewModel.TaskTableAttributeWrappers.Add(taskTableAttributeWrapper);
            }
        }
    }
}