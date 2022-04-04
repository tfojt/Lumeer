using Lumeer.Models.Rest;
using Lumeer.Services;
using Lumeer.Utils;
using Lumeer.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Lumeer.ViewModels
{
    public class TasksViewModel : BaseViewModel
    {
        private readonly IAlertService _alertService;
        private readonly INavigationService _navigationService;

        public ObservableCollection<Models.Rest.Task> Tasks { get; set; } = new ObservableCollection<Models.Rest.Task>();

        public ICommand RefreshTasksCmd { get; set; }
        public ICommand SearchCmd { get; set; }
        public ICommand SearchSettingsCmd { get; set; }
        public ICommand CreateTaskCmd { get; set; }

        private bool _isRefreshingTasks;
        public bool IsRefreshingTasks 
        {
            get => _isRefreshingTasks;
            set => SetValue(ref _isRefreshingTasks, value); 
        }

        private Models.Rest.Task _selectedTask;
        public Models.Rest.Task SelectedTask 
        {
            get => _selectedTask;
            set
            {
                SetValue(ref _selectedTask, value);
                if (value != null)
                {
                    DisplayTaskDetail(value);
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
            _alertService = DependencyService.Get<IAlertService>();
            _navigationService = DependencyService.Get<INavigationService>();
            RefreshTasksCmd = new Command(RefreshTasks);
            SearchCmd = new Command(Search);
            SearchSettingsCmd = new Command(DisplaySearchSettings);
            CreateTaskCmd = new Command(CreateTask);

            Init();
        }

        private async void DisplayTaskDetail(Models.Rest.Task task)
        {
            // TODOT cache LastTaskDetail and unhook TaskChangesSaved event?
            
            var table = Session.Instance.AllTables.Single(t => t.Id == task.CollectionId);
            var taskDetailPage = new TaskDetailPage(task, table);
            taskDetailPage.TaskDetailViewModel.TaskChangesSaved += TaskDetailViewModel_TaskChangesSaved;

            await _navigationService.PushAsync(taskDetailPage);
            SelectedTask = null;
        }

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
            // TODOT filter
        }

        private async void Init()
        {
            try
            {
                Session.Instance.User = await GetUser();
                Session.Instance.OrganizationId = Session.Instance.User.DefaultWorkspace.OrganizationId;
                Session.Instance.ProjectId = Session.Instance.User.DefaultWorkspace.ProjectId;
                Session.Instance.AllTables = await GetTables();
                Session.Instance.Organizations = await GetOrganizations();
                Session.Instance.Users = await GetUsers();
                RefreshTasks();
            }
            catch (Exception ex)
            {
                var message = "Could not obtain data from server";
                await ProcessException(message, ex);
                // TODOT Exit app?
            }
        }

        private async void RefreshTasks()
        {
            try
            {
                var tasks = await GetTasks();
                //LoadedTasks?.Invoke(tasks);
                UpdateTasksDataTemplate();

                Tasks.Clear();
                foreach (var task in tasks.Documents)
                {
                    Tasks.Add(task);
                }
            }
            catch (Exception ex)
            {
                var message = "Could not refresh tasks";
                _ = ProcessException(message, ex);
            }
            finally
            {
                IsRefreshingTasks = false;
            }
        }

        private void UpdateTasksDataTemplate()
        {
            TasksDataTemplate = new DataTemplate(() =>
            {
                var stackLayout = new StackLayout();
                // TODOT kolik se jich tady realne ukazuje? nebo se ukazuji jen pokud jsou tam Assignee, DueDate,...?
                int propertiesCount = 4;
                for (int i = 1; i < propertiesCount + 1; i++)
                {
                    var label = new Label();
                    label.SetBinding(Label.TextProperty, $"Data[a{i}]", BindingMode.TwoWay);
                    stackLayout.Children.Add(label);
                }

                // TODOT add menu item
                /*
                <ViewCell.ContextActions>
                                <MenuItem Text="Favorite" 
                                          Command="{Binding FavoriteCmd}" 
                                          CommandParameter="{Binding .}"/>
                            </ViewCell.ContextActions> 
                */

                var viewCell = new ViewCell 
                { 
                    View = stackLayout 
                };

                return viewCell;

                /*var grid = new Grid();
                var nameLabel = new Label { FontAttributes = FontAttributes.Bold };
                var ageLabel = new Label();
                var locationLabel = new Label { HorizontalTextAlignment = TextAlignment.End };

                nameLabel.SetBinding(Label.TextProperty, "Data[a1]");
                ageLabel.SetBinding(Label.TextProperty, "Data[a2]");
                locationLabel.SetBinding(Label.TextProperty, "Data[a3]");

                grid.Children.Add(nameLabel);
                grid.Children.Add(ageLabel, 1, 0);
                grid.Children.Add(locationLabel, 2, 0);

                return new ViewCell { View = grid };*/
            });
        }

        private async System.Threading.Tasks.Task ProcessException(string message, Exception ex)
        {
            Debug.WriteLine($"{message}: {ex}");
            await _alertService.DisplayAlert("Error", message, "Ok");
        }

        // TODOT create utility that shows activity indicator when loading data from api?
        private async Task<User> GetUser()
        {
            string uri = "users/current";
            return await ApiClient.Instance.SendRequestGetContent<User>(HttpMethod.Get, uri);
        }

        private async Task<List<Table>> GetTables()
        {
            string uri = $"organizations/{Session.Instance.OrganizationId}/projects/{Session.Instance.ProjectId}/collections";
            return await ApiClient.Instance.SendRequestGetContent<List<Table>>(HttpMethod.Get, uri);
        }
        
        private async Task<List<User>> GetUsers()
        {
            string uri = $"users/organizations/{Session.Instance.OrganizationId}/users";
            return await ApiClient.Instance.SendRequestGetContent<List<User>>(HttpMethod.Get, uri);
        }
        
        private async Task<List<Organization>> GetOrganizations()
        {
            string uri = "organizations";
            return await ApiClient.Instance.SendRequestGetContent<List<Organization>>(HttpMethod.Get, uri);
        }

        private async Task<Tasks> GetTasks()
        {
            string uri = $"organizations/{Session.Instance.OrganizationId}/projects/{Session.Instance.ProjectId}/search/tasks?subItems=false";
            var searchFilter = new SearchFilter();  // TODOT take this from SearchBar
            return await ApiClient.Instance.SendRequestGetContent<Tasks>(HttpMethod.Post, uri, searchFilter);
        }
    }
}
