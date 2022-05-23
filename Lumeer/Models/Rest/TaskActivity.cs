using System;
using System.Collections.Generic;
using System.Text;

namespace Lumeer.Models.Rest
{
    /*{
    "parentId": "621fb8f9d508224e0888e794",
    "resourceType": "DOCUMENT",
    "resourceId": "621fb8f9d508224e0888e7a0",
    "changeDate": 1651770747335,
    "user": "620fa0c48e43bf296c088962",
    "userName": "Tomas Fojt",
    "userEmail": "tomasfojt@seznam.cz",
    "oldState": { "a3": "open" },
    "newState": { "a3": "Open" },
    "id": "6274057b47fe0b7f53194194",
    "viewId": "",
    "type": "Updated"
  }*/
    public class TaskActivity
    {
        public string ParentId { get; set; }
        public string ResourceType { get; set; }
        public string ResourceId { get; set; }
        public long ChangeDate { get; set; }
        public string User { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public Dictionary<string, object> OldState { get; set; }
        public Dictionary<string, object> NewState { get; set; }
        public string Id { get; set; }
        public string ViewId { get; set; }
        public string Type { get; set; }
    }
}
