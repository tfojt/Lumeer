using Lumeer.Models;
using Lumeer.Models.Rest;
using Lumeer.ViewModels;
using System.Collections.Generic;
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
            InitializeComponent();

            TaskDetailViewModel = new TaskDetailViewModel(task, table, taskTableView.TableSection);
            BindingContext = TaskDetailViewModel;


            /*foreach (var taskTableAttributeWrapper in TaskDetailViewModel.TaskTableAttributeWrappers)
            {
                tableSection.Add(taskTableAttributeWrapper.Cell);
                //taskTableView.TableSection.Add(taskTableAttributeWrapper.Cell);
            }*/

            /*foreach (TableAttribute tableAttribute in table.Attributes)
            {
                var taskTableAttributeWrapper = new TaskTableAttributeWrapper(task, tableAttribute);
                tableSection.Add(taskTableAttributeWrapper.Cell);
                TaskDetailViewModel.TaskTableAttributeWrappers.Add(taskTableAttributeWrapper);
            }*/
        }

        /*private void TaskTableAttributeWrappersCreated(List<TaskTableAttributeWrapper> taskTableAttributeWrappers)
        {
            taskTableView.TableSection.Clear();

            foreach (var taskTableAttributeWrapper in taskTableAttributeWrappers)
            {
                taskTableView.TableSection.Add(taskTableAttributeWrapper.Cell);
            }
        }*/
    }
}