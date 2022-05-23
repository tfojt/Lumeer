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

        private List<Models.Rest.Task> _originalTasks = new List<Models.Rest.Task>();
        public List<Models.Rest.Task> OriginalTasks 
        {
            get => _originalTasks;
            set
            {
                _originalTasks = value;

                FilterDisplayedTasks();
            }
        }

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

        public IAsyncCommand RefreshTasksCmd { get; set; }
        public ICommand SearchCmd { get; set; }
        public ICommand SearchSettingsCmd { get; set; }
        public ICommand CreateTaskCmd { get; set; }
        public ICommand ChangeTaskFavoriteStatusCmd { get; set; }

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

        private DataTemplate _tasksDataTemplate;
        public DataTemplate TasksDataTemplate
        {
            get => _tasksDataTemplate;
            set => SetValue(ref _tasksDataTemplate, value);
        }

        public TasksViewModel()
        {
            _navigationService = DependencyService.Get<INavigationService>();

            RefreshTasksCmd = new AsyncCommand(RefreshTasks, allowsMultipleExecutions: false);
            SearchCmd = new Command(Search);
            SearchSettingsCmd = new Command(DisplaySearchSettings);
            CreateTaskCmd = new Command(CreateTask);
            ChangeTaskFavoriteStatusCmd = new Command<TaskItem>(ChangeTaskFavoriteStatus);

            Task.Run(RefreshTasks);
        }

        private Dictionary<string, DataTemplate> CreateTableDataTemplates()
        {
            var tableDataTemplates = new Dictionary<string, DataTemplate>();

            foreach (var table in Session.Instance.TaskTables)
            {
                /*foreach (TableAttribute tableAttribute in table.Attributes)
                {
                    var constraint = tableAttribute.Constraint;
                    if (constraint == null)
                    {
                        return AttributeType.None;
                    }

                    return (AttributeType)Enum.Parse(typeof(AttributeType), constraint.Type);
                }*/

                /*var favoriteSwipeItem = new SwipeItem()
                {
                    BackgroundColor = Color.Orange,
                    Command = ChangeTaskFavoriteStatusCmd,
                };
                favoriteSwipeItem.SetBinding(MenuItem.TextProperty, "FavoriteText");
                favoriteSwipeItem.SetBinding(MenuItem.CommandParameterProperty, ".");

                var dataTemplate = new DataTemplate(() =>
                {
                    var stackLayout = new StackLayout();

                    var swipeView = new SwipeView()
                    {
                        RightItems = new SwipeItems()
                        {
                            favoriteSwipeItem
                        },
                    };

                    int propertiesCount = 4;
                    for (int i = 1; i < propertiesCount + 1; i++)
                    {
                        var label = new Label();
                        label.SetBinding(Label.TextProperty, $"Data[a{i}]", BindingMode.TwoWay);
                        stackLayout.Children.Add(label);
                    }

                    swipeView.Content = stackLayout;

                    return swipeView;
                });*/

                var dataTemplate = new DataTemplate(() =>
                {
                    var stackLayout = new StackLayout()
                    {
                        Orientation = StackOrientation.Horizontal
                    };

                    // TODOT replace with icon
                    var boxView = new BoxView()
                    {
                        Color = Color.FromHex(table.Color),
                        WidthRequest = 35,
                        HeightRequest = 35,
                    };
                    stackLayout.Children.Add(boxView);

                    // TODOT find out how to display "Empty title" if value is null
                    var label = new Label()
                    {
                        Margin = new Thickness(8, 0, 0, 0),
                        VerticalOptions = LayoutOptions.Center
                    };
                    label.SetBinding(Label.TextProperty, $"Data[a{1}]", BindingMode.TwoWay);
                    stackLayout.Children.Add(label);

                    var menuItem = new MenuItem()
                    {
                        Command = ChangeTaskFavoriteStatusCmd,
                    };
                    menuItem.SetBinding(MenuItem.TextProperty, "FavoriteText");
                    menuItem.SetBinding(MenuItem.CommandParameterProperty, ".");

                    var viewCell = new ViewCell
                    {
                        View = new Frame()
                        {
                            CornerRadius = 10,
                            HasShadow = true,
                            Content = stackLayout
                        },
                        ContextActions =
                        {
                            menuItem
                        }
                    };

                    return viewCell;
                });

                tableDataTemplates.Add(table.Id, dataTemplate);
            }

            return tableDataTemplates;
        }

        private async void DisplayTaskOverview(TaskItem taskItem)
        {
            var task = taskItem.Task;
            var table = Session.Instance.AllTables.Single(t => t.Id == task.CollectionId);

            // TODOT save changes made in TaskDetailView

            /*var taskDetailPage = new TaskDetailPage(task, table);
            taskDetailPage.TaskDetailViewModel.TaskChangesSaved += TaskDetailViewModel_TaskChangesSaved;*/

            var taskOverviewPage = new TaskOverviewPage(task, table);
            taskOverviewPage.TaskOverviewViewModel.TaskDeleted += TaskOverviewViewModel_TaskDeleted;
            await _navigationService.PushAsync(taskOverviewPage);
            SelectedTask = null;
        }

        private void TaskOverviewViewModel_TaskDeleted(Models.Rest.Task task)
        {
            var taskItem = DisplayedTasks.Single(t => t.Task == task);
            DisplayedTasks.Remove(taskItem);

            OriginalTasks.Remove(task);
        }

        /*private async void DisplayTaskDetail(TaskItem taskItem)
        {
            // TODOT cache LastTaskDetail and unhook TaskChangesSaved event?

            var task = taskItem.Task;
            var table = Session.Instance.AllTables.Single(t => t.Id == task.CollectionId);
            var taskDetailPage = new TaskDetailPage(task, table);
            taskDetailPage.TaskDetailViewModel.TaskChangesSaved += TaskDetailViewModel_TaskChangesSaved;

            await _navigationService.PushAsync(taskDetailPage);
            SelectedTask = null;
        }*/

        private void TaskDetailViewModel_TaskChangesSaved(Models.Rest.Task task)
        {
            /*// TODOT make binding work
            var taskIndex = Tasks.IndexOf(task);
            Tasks.Remove(task);
            Tasks.Insert(taskIndex, task);*/
            RefreshTasks();
        }

        private void CreateTask()
        {
            var newTaskPage = new NewTaskPage();
            newTaskPage.NewTaskViewModel.TaskCreated += NewTaskViewModel_TaskCreated;
            _navigationService.PushModalAsync(newTaskPage);
        }

        private void NewTaskViewModel_TaskCreated(Models.Rest.Task task)
        {
            /*// TODOT 
            Tasks.Add(task);*/
            RefreshTasks();
        }

        private void DisplaySearchSettings()
        {
            _navigationService.PushAsync(new SearchSettingsPage());
            // TODOT apply settings
        }

        private void Search()
        {
            FilterDisplayedTasks();
        }

        private void FilterDisplayedTasks()
        {
            DisplayedTasks.Clear();

            if (string.IsNullOrEmpty(SearchedText))
            {
                foreach (var task in OriginalTasks)
                {
                    var taskItem = new TaskItem(task);
                    DisplayedTasks.Add(taskItem);
                }
            }
            else
            {
                foreach (var task in OriginalTasks)
                {
                    foreach (object value in task.Data.Values)
                    {
                        if (value == null)
                        {
                            continue;
                        }

                        string stringValue = value.ToString();
                        if (stringValue.Contains(SearchedText))
                        {
                            var taskItem = new TaskItem(task);
                            DisplayedTasks.Add(taskItem);
                            break;
                        }
                    }
                }
            }
        }

        private async Task RefreshTasks()
        {
            IsRefreshingTasks = true;

            try
            {
                var tasks = await GetTasks();
                OriginalTasks = tasks.Documents;
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
        
        private async Task<Tasks> GetTasks()
        {
            var searchFilter = new SearchFilter();  // TODOT take this from SearchSettings
            return await ApiClient.Instance.GetTasks(searchFilter);
        }

        private async void ChangeTaskFavoriteStatus(TaskItem taskItem)
        {
            await ChangeTaskFavoriteStatus(taskItem.Task);
        }
    }
}
