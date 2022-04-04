using Lumeer.Models.Rest;
using System.Collections.Generic;
using System.Linq;

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

        static Session()
        {
            Instance = new Session();
        }
    }
}
