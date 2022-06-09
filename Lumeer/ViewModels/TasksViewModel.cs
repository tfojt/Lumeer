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

        public ObservableCollection<TaskGroup> DisplayedTaskGroups { get; set; } = new ObservableCollection<TaskGroup>();

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

            foreach (var taskGroup in DisplayedTaskGroups)
            {
                for (int i = 0; i < taskGroup.Count; i++)
                {
                    var currentTaskItem = taskGroup[i];
                    if (currentTaskItem == taskItem)
                    {
                        taskGroup.RemoveAt(i);
                        return;
                    }
                }
            }
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
            tasksFilterPage.TasksFilterViewModel.GroupByChanged += TasksFilterViewModel_GroupByChanged;

            _navigationService.PushAsync(tasksFilterPage);
        }

        private void TasksFilterViewModel_GroupByChanged()
        {
            FilterDisplayedTasks();
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
            DisplayedTaskGroups.Clear();

            bool shouldSearch = !string.IsNullOrEmpty(SearchedText);

            var tasksToDisplay = new List<TaskItem>();
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

                    tasksToDisplay.Add(taskItem);
                    break;
                }
            }

            if (!Session.Instance.SearchConfig.Config.Search.Documents.GroupBy.HasValue)
            {
                var taskGroup = new TaskGroup(TaskGroup.ALL_GROUP_NAME, tasksToDisplay);
                DisplayedTaskGroups.Add(taskGroup);
            }
            else
            {
                var othersGroup = new TaskGroup(TaskGroup.OTHERS_GROUP_NAME);

                foreach (var taskItem in tasksToDisplay)
                {
                    if (TryGetGroupName(taskItem, out string groupName))
                    {
                        var group = DisplayedTaskGroups.SingleOrDefault(g => g.Name == groupName);
                        if (group == null)
                        {
                            group = new TaskGroup(groupName);
                            DisplayedTaskGroups.Add(group);
                        }

                        group.Add(taskItem);
                    }
                    else
                    {
                        othersGroup.Add(taskItem);
                    }
                }

                // So 'Others' group is displayed as last group
                if (othersGroup.Any())
                {
                    DisplayedTaskGroups.Add(othersGroup);
                }
            }
        }

        private bool TryGetGroupName(TaskItem taskItem, out string groupName)
        {
            var table = Session.Instance.TaskTables.Single(t => t.Id == taskItem.Task.CollectionId);

            string groupByAttributeId = null;
            switch (Session.Instance.SearchConfig.Config.Search.Documents.GroupBy.Value)
            {
                case Models.Rest.Enums.TaskTableAttribute.Assignee:
                    groupByAttributeId = "assigneeAttributeId";
                    break;
                case Models.Rest.Enums.TaskTableAttribute.DueDate:
                    groupByAttributeId = "dueDateAttributeId";
                    break;
                case Models.Rest.Enums.TaskTableAttribute.State:
                    groupByAttributeId = "stateAttributeId";
                    break;
                case Models.Rest.Enums.TaskTableAttribute.Priority:
                    groupByAttributeId = "priorityAttributeId";
                    break;
                default:
                    break;
            }

            string taskAttributeId = (string)table.PurposeMetaData[groupByAttributeId];
            if (taskAttributeId == null || !taskItem.Task.Data.TryGetValue(taskAttributeId, out var value))
            {
                groupName = null;
                return false;
            }

            groupName = value?.ToString();
            if (string.IsNullOrEmpty(groupName))
            {
                groupName = null;
                return false;
            }

            return true;
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

                await Device.InvokeOnMainThreadAsync(FilterDisplayedTasks);
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
