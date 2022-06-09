using Lumeer.Models;
using Lumeer.Models.Rest;
using Lumeer.Models.Rest.Enums;
using Lumeer.Services;
using Lumeer.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static Lumeer.Utils.EventHandlers;
using Task = System.Threading.Tasks.Task;

namespace Lumeer.ViewModels
{
    public class TasksFilterViewModel : BaseViewModel
    {
        public event EmptyEventHandler TasksFilterChanged;
        public event EmptyEventHandler GroupByChanged;

        private readonly INavigationService _navigationService;
        private readonly IAlertService _alertService;

        public ObservableCollection<Organization> Organizations { get; set; } = new ObservableCollection<Organization>();

        private Organization _selectedOrganization;
        public Organization SelectedOrganization 
        {
            get => _selectedOrganization;
            set
            {
                bool valueChanged = SetValue(ref _selectedOrganization, value);
                if (!valueChanged)
                {
                    return;
                }

                SelectedOrganizationChanged();
            }
        }

        public ObservableCollection<Project> Projects { get; set; } = new ObservableCollection<Project>();

        private Project _selectedProject;
        public Project SelectedProject
        {
            get => _selectedProject;
            set
            {
                bool valueChanged = SetValue(ref _selectedProject, value);
                if (!valueChanged)
                {
                    return;
                }

                SelectedProjectChanged();
            }
        }

        private bool _canSelectProject = true;
        public bool CanSelectProject 
        {
            get => _canSelectProject;
            set => SetValue(ref _canSelectProject, value);
        }
        
        public ObservableCollection<Table> Tables { get; set; } = new ObservableCollection<Table>();

        private Table _selectedTable;
        public Table SelectedTable
        {
            get => _selectedTable;
            set
            {
                IsTableSelected = value != null;

                bool valueChanged = SetValue(ref _selectedTable, value);
                if (!valueChanged)
                {
                    return;
                }

                Session.Instance.CurrentTaskTable = _selectedTable;
            }
        }

        private bool _canSelectTable = true;
        public bool CanSelectTable
        {
            get => _canSelectTable;
            set => SetValue(ref _canSelectTable, value);
        }
        
        private bool _isTableSelected;
        public bool IsTableSelected
        {
            get => _isTableSelected;
            set => SetValue(ref _isTableSelected, value);
        }
        
        private bool _includeSubItems;
        public bool IncludeSubItems
        {
            get => _includeSubItems;
            set => SetValue(ref _includeSubItems, value);
        }

        public List<TaskTableAttribute> GroupByAttributes { get; set; }

        private TaskTableAttribute _originalGroupByAttribute;

        public TaskTableAttribute SelectedGroupByAttribute { get; set; }

        public ICommand ClearSelectedTableCmd => new Command(ClearSelectedTable);

        private TasksFilterSettings _tasksFilterSettings;
        private bool _workspaceChanged;

        public TasksFilterViewModel(TasksFilterSettings tasksFilterSettings)
        {
            _tasksFilterSettings = tasksFilterSettings;

            _navigationService = DependencyService.Get<INavigationService>();
            _alertService = DependencyService.Get<IAlertService>();

            Organizations.AddRange(Session.Instance.Organizations);
            _selectedOrganization = Session.Instance.CurrentOrganization;

            Projects.AddRange(Session.Instance.Projects);
            _selectedProject = Session.Instance.CurrentProject;

            Tables.AddRange(Session.Instance.TaskTables);
            _selectedTable = Session.Instance.CurrentTaskTable;

            IncludeSubItems = tasksFilterSettings.IncludeSubItems;

            GroupByAttributes = new List<TaskTableAttribute>
            {
                TaskTableAttribute.None,
                TaskTableAttribute.Assignee,
                TaskTableAttribute.DueDate,
                TaskTableAttribute.Priority,
                TaskTableAttribute.State
            };

            _originalGroupByAttribute = Session.Instance.SearchConfig.Config.Search.Documents.GroupBy ?? TaskTableAttribute.None;
            SelectedGroupByAttribute = _originalGroupByAttribute;
        }

        private async void SelectedOrganizationChanged()
        {
            CanSelectProject = false;
            try
            {
                Session.Instance.CurrentOrganization = _selectedOrganization;
                await Session.Instance.LoadUsers();
                await Session.Instance.LoadSelectionLists();
                await LoadProjects();
            }
            catch (Exception ex)
            {
                await _alertService.DisplayAlert("Error", "Sorry, there was an error while changing organization", "Ok", ex);
            }
            finally
            {
                CanSelectProject = true;
            }
        }

        private async Task LoadProjects()
        {
            await Session.Instance.LoadProjects();
            Projects.AddRange(Session.Instance.Projects, true);

            SelectedProject = Projects.First();
        }

        private async void SelectedProjectChanged()
        {
            CanSelectTable = false;
            try
            {
                Session.Instance.CurrentProject = _selectedProject;
                await ApiClient.Instance.ChangeWorkspace();
                _workspaceChanged = true;

                await Session.Instance.LoadLinkTypes();
                await Session.Instance.LoadConfigs();
                await LoadTables();
            }
            catch (Exception ex)
            {
                await _alertService.DisplayAlert("Error", "Sorry, there was an error while changing project", "Ok", ex);
            }
            finally
            {
                CanSelectTable = true;
            }
        }
        
        private async Task LoadTables()
        {
            await Session.Instance.LoadTables();
            Tables.AddRange(Session.Instance.TaskTables, true);

            SelectedTable = null;
        }

        private void ClearSelectedTable()
        {
            SelectedTable = null;
        }

        public async Task CheckTasksFilterSettingsChanged()
        {
            bool includeSubItemsChanged = CheckIncludeSubItemsChanged();

            bool tableChanged = CheckSelectedTableChanged();

            bool groupByChanged = await CheckSelectedGroupByAttributeChanged();

            if (includeSubItemsChanged || tableChanged || _workspaceChanged)
            {
                TasksFilterChanged?.Invoke();
            }
            else if (groupByChanged)
            {
                GroupByChanged.Invoke();
            }
        }

        private bool CheckIncludeSubItemsChanged()
        {
            if (_tasksFilterSettings.IncludeSubItems == IncludeSubItems)
            {
                return false;
            }

            _tasksFilterSettings.IncludeSubItems = IncludeSubItems;
            return true;
        }

        private bool CheckSelectedTableChanged()
        {
            if (!IsTableSelected)
            {
                bool wasTableSelected = _tasksFilterSettings.TasksFilter.Stems.Count != 0;
                if (!wasTableSelected)
                {
                    return false;
                }

                _tasksFilterSettings.TasksFilter.Stems.Clear();
            }
            else
            {
                bool sameTableSelected = _tasksFilterSettings.TasksFilter.Stems.Any(s => s.CollectionId == _selectedTable.Id);
                if (sameTableSelected)
                {
                    return false;
                }

                _tasksFilterSettings.TasksFilter.Stems.Clear();

                var stem = new Stem(_selectedTable);
                _tasksFilterSettings.TasksFilter.Stems.Add(stem);
            }

            return true;
        }

        private async Task<bool> CheckSelectedGroupByAttributeChanged()
        {
            if (_originalGroupByAttribute == SelectedGroupByAttribute)
            {
                return false;
            }

            Session.Instance.SearchConfig.Config.Search.Documents.GroupBy = SelectedGroupByAttribute != TaskTableAttribute.None ?
                    (TaskTableAttribute?)SelectedGroupByAttribute :
                    null;

            try
            {
                await ApiClient.Instance.ChangeSearchConfig(Session.Instance.SearchConfig);
            }
            catch (Exception ex)
            {
                await _alertService.DisplayAlert("Error", "Sorry, there was an error while changing Group By attribute", "Ok", ex);
                return false;
            }

            return true;
        }
    }
}
