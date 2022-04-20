using Lumeer.Models.Rest;
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
                _taskTables = _allTables.Where(t => t.PurposeType == PurposeType.Tasks).ToList();
            }
        }

        private List<Table> _taskTables;
        public List<Table> TaskTables => _taskTables;

        public List<Organization> Organizations { get; set; }

        public string OrganizationId { get; set; }
        public string ProjectId { get; set; }

        public List<User> Users { get; set; }

        public List<SelectionList> SelectionLists { get; set; }

        static Session()
        {
            Instance = new Session();
        }

        public async Task LoadUsersInitialData()
        {
            User = await ApiClient.Instance.GetUser();
            OrganizationId = User.DefaultWorkspace.OrganizationId;
            ProjectId = User.DefaultWorkspace.ProjectId;
            AllTables = await ApiClient.Instance.GetTables();
            Organizations = await ApiClient.Instance.GetOrganizations();
            Users = await ApiClient.Instance.GetUsers();
            SelectionLists = await ApiClient.Instance.GetSelectionLists();
        }
    }
}
