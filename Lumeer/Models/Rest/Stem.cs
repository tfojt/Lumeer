using System;
using System.Collections.Generic;
using System.Text;

namespace Lumeer.Models.Rest
{
    /*
    {
      "collectionId": "62402061a26fa76666627730",
      "documentIds": [],
      "linkTypeIds": [],
      "filters": [],
      "linkFilters": []
    }
     */
    public class Stem
    {
        public string CollectionId { get; set; }
        public List<object> DocumentIds { get; set; } = new List<object>();
        public List<object> LinkTypeIds { get; set; } = new List<object>();
        public List<object> Filters { get; set; } = new List<object>();
        public List<object> LinkFilters { get; set; } = new List<object>();
    }
}
