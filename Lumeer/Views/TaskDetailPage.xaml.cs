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
        public TaskDetailPage(Task task, Table table)
        {
            var taskDetailViewModel = new TaskDetailViewModel(task, table);
            BindingContext = taskDetailViewModel;

            InitializeComponent();

            foreach (TableAttribute tableAttribute in table.Attributes)
            {
                var taskTableAttributeWrapper = new TaskTableAttributeWrapper(task, tableAttribute);
                tableSection.Add(taskTableAttributeWrapper.Cell);
                taskDetailViewModel.TaskTableAttributeWrappers.Add(taskTableAttributeWrapper);
            }
        }
    }
}