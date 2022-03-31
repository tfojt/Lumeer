using Lumeer.Models.Rest;
using System.Collections.Generic;

namespace Lumeer.Utils
{
    public sealed class Session
    {
        public static Session Instance { get; }

        public User User { get; set; }
        public List<Table> Tables { get; set; }
        public string OrganizationId { get; set; }
        public string ProjectId { get; set; }

        static Session()
        {
            Instance = new Session();
        }
    }
}
