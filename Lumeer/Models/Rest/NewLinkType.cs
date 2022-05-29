using System;
using System.Collections.Generic;
using System.Text;

namespace Lumeer.Models.Rest
{
    public class NewLinkType
    {
        public string Name { get; set; }
        public string CurrentTableId { get; set; }
        public string LinkedTableId { get; set; }

        public NewLinkType(string name, string currentTableId, string linkedTableId)
        {
            Name = name;
            CurrentTableId = currentTableId;
            LinkedTableId = linkedTableId;
        }
    }
}
