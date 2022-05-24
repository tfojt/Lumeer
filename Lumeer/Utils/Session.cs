using Lumeer.Models.Rest;
using Lumeer.Models.Rest.Enums;
using System.Collections.Generic;
using System.Linq;
using Task = System.Threading.Tasks.Task;

namespace Lumeer.Utils
{
    public sealed class Session
    {
        public static Session Instance { get; }

        public User User { get; set; }

        private List<Table> _allTables;
        public List<Table> AllTables 
        {
            get => _allTables;
            set
            {
                _allTables = value;
                _taskTables = _allTables.Where(t => t.PurposeType == CollectionPurposeType.Tasks).ToList();
            }
        }

        private List<Table> _taskTables;
        public List<Table> TaskTables => _taskTables;
        public Table CurrentTaskTable { get; set; }

        public List<Organization> Organizations { get; set; }
        public Organization CurrentOrganization { get; set; }

        public List<Project> Projects { get; set; }
        public Project CurrentProject { get; set; }

        public List<User> Users { get; set; }

        public List<SelectionList> SelectionLists { get; set; }

        static Session()
        {
            Instance = new Session();
        }

        public async Task LoadUsersInitialData()
        {
            User = await ApiClient.Instance.GetUser();

            await LoadOrganizations();
            CurrentOrganization = Organizations.Single(o => o.Id == User.DefaultWorkspace.OrganizationId);

            await LoadProjects();
            CurrentProject = Projects.Single(p => p.Id == User.DefaultWorkspace.ProjectId);

            await LoadTables();
            await LoadUsers();
            await LoadSelectionLists();
        }

        public async Task LoadOrganizations()
        {
            Organizations = await ApiClient.Instance.GetOrganizations();
        }

        public async Task LoadProjects()
        {
            Projects = await ApiClient.Instance.GetProjects();
        }

        public async Task LoadTables()
        {
            AllTables = await ApiClient.Instance.GetTables();
        }

        public async Task LoadUsers()
        {
            Users = await ApiClient.Instance.GetUsers();
        }

        public async Task LoadSelectionLists()
        {
            SelectionLists = await ApiClient.Instance.GetSelectionLists();
        }
    }
}
