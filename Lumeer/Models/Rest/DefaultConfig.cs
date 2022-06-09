using Lumeer.Models.Rest.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lumeer.Models.Rest
{
    /*
    {
    "key": "default",
    "perspective": "search",
    "config": {
      "search": {
        "searchTab": "tasks",
        "documents": {
          "size": "S",
          "expandedIds": [ "621fb8f9d508224e0888e799" ],
          "groupBy": "priority",
          "sortBy": [
            {
              "attribute": "priority",
              "type": "desc"
            },
            { "attribute": "dueDate" },
            { "attribute": "lastUsed" }
          ]
        },
        "views": { "size": "S" }
      }
    },
    "updatedAt": 1654443847756,
    "userId": "620fa0c48e43bf296c088962"
    }
    */
    public class DefaultConfig
    {
        public string Key { get; set; }
        public PerspectiveType Perspective { get; set; }
        public object Config { get; set; }
        public long UpdatedAt { get; set; }
        public string UserId { get; set; }
    }
}
