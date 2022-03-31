using System;
using System.Collections.Generic;
using System.Text;

namespace Lumeer.Models.Rest
{
    public class AttributeConstraint
    {
        public string Type { get; set; }
        public Dictionary<string, object> Config { get; set; }
    }
}
