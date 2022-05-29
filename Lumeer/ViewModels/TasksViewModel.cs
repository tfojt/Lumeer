using Lumeer.Models;
using Lumeer.Models.Rest;
using Lumeer.Services;
using Lumeer.TemplateSelectors;
using Lumeer.Utils;
using Lumeer.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Task = System.Threading.Tasks.Task;

namespace Lumeer.ViewModels
{
    public class TasksViewModel : BaseTaskViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IAlertService _alertService;

        public List<TaskItem> OriginalTasks { get; set; } = new List<TaskItem>();

        public ObservableCollection<TaskItem> DisplayedTasks { get; set; } = new ObservableCollection<TaskItem>();

        private string _searchedText;
        public string SearchedText
        {
            get => _searchedText;
            set
            {
                SetValue(ref _searchedText, value);

                if (string.IsNullOrEmpty(_searchedText))    // text is cleared and we have to manually show all tasks, because search is not triggered when the text is empty
                {
                    FilterDisplayedTasks();
                }
            }
        }

        public IAsyncCommand RefreshTasksCmd => new AsyncCommand(RefreshTasks, allowsMultipleExecutions: false);
        public ICommand SearchCmd => new Command(Search);
        public ICommand TasksFilterCmd => new Command(DisplayTasksFilter);
        public ICommand CreateTaskCmd => new Command(CreateTask);
        public ICommand ChangeTaskFavoriteStatusCmd => new Command<TaskItem>(ChangeTaskFavoriteStatus);

        private bool _isRefreshingTasks;
        public bool IsRefreshingTasks 
        {
            get => _isRefreshingTasks;
            set => SetValue(ref _isRefreshingTasks, value); 
        }

        private TaskItem _selectedTask;
        public TaskItem SelectedTask 
        {
            get => _selectedTask;
            set
            {
                SetValue(ref _selectedTask, value);
                if (value != null)
                {
                    DisplayTaskOverview(value);
                }
            }
        }

        private TasksFilterSettings _tasksFilterSettings = new TasksFilterSettings();

        public TasksViewModel()
        {
            _navigationService = DependencyService.Get<INavigationService>();
            _alertService = DependencyService.Get<IAlertService>();

            Task.Run(RefreshTasks);
        }

        private async void DisplayTaskOverview(TaskItem taskItem)
        {
            var task = taskItem.Task;
            var table = Session.Instance.AllTables.Single(t => t.Id == task.CollectionId);

            var taskOverviewPage = new TaskOverviewPage(task, table);
            taskOverviewPage.TaskOverviewViewModel.TaskDeleted += TaskOverviewViewModel_TaskDeleted;
            taskOverviewPage.TaskDetailView.TaskDetailViewModel.TaskChangesSaved += TaskDetailViewModel_TaskChangesSaved;
            await _navigationService.PushAsync(taskOverviewPage);
            SelectedTask = null;
        }

        private void TaskOverviewViewModel_TaskDeleted(Models.Rest.Task task)
        {
            var taskItem = OriginalTasks.Single(t => t.Task == task);
            OriginalTasks.Remove(taskItem);
            DisplayedTasks.Remove(taskItem);
        }

        private async void TaskDetailViewModel_TaskChangesSaved(Models.Rest.Task task)
        {
            await RefreshTasks();
        }

        private void CreateTask()
        {
            if (!Session.Instance.TaskTables.Any())
            {
                _alertService.DisplayAlert("Warning", "You have no task tables!", "Ok");
                return;
            }

            var newTaskPage = new NewTaskPage();
            newTaskPage.NewTaskViewModel.TaskCreated += NewTaskViewModel_TaskCreated;
            _navigationService.PushModalAsync(newTaskPage);
        }

        private async void NewTaskViewModel_TaskCreated(Models.Rest.Task task)
        {
            await RefreshTasks();
        }

        private void DisplayTasksFilter()
        {
            var tasksFilterPage = new TasksFilterPage(_tasksFilterSettings);
            tasksFilterPage.TasksFilterViewModel.TasksFilterChanged += TasksFilterViewModel_TasksFilterChanged;

            _navigationService.PushAsync(tasksFilterPage);
        }

        private async void TasksFilterViewModel_TasksFilterChanged()
        {
            await RefreshTasks();
        }

        private void Search()
        {
            FilterDisplayedTasks();
        }

        private void FilterDisplayedTasks()
        {
            DisplayedTasks.Clear();

            bool shouldSearch = !string.IsNullOrEmpty(SearchedText);

            foreach (var taskItem in OriginalTasks)
            {
                foreach (object value in taskItem.Task.Data.Values)
                {
                    if (value == null)
                    {
                        continue;
                    }

                    if (shouldSearch && !value.ToString().Contains(SearchedText))
                    {
                        continue;
                    }

                    DisplayedTasks.Add(taskItem);
                    break;
                }
            }
        }

        private async Task RefreshTasks()
        {
            IsRefreshingTasks = true;

            try
            {
                var tasks = await ApiClient.Instance.GetTasks(_tasksFilterSettings);

                OriginalTasks.Clear();

                foreach (var task in tasks.Documents)
                {
                    var taskItem = new TaskItem(task);
                    OriginalTasks.Add(taskItem);
                }

                FilterDisplayedTasks();
            }
            catch (Exception ex)
            {
                var message = "Could not refresh tasks";
                await AlertService.DisplayAlert("Error", message, "Ok", ex);
            }
            finally
            {
                IsRefreshingTasks = false;
            }
        }

        private async void ChangeTaskFavoriteStatus(TaskItem taskItem)
        {
            await ChangeTaskFavoriteStatus(taskItem.Task);
        }
    }
}
