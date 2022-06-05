using Lumeer.Models.Rest.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lumeer.Models.Rest
{
    public class AttributeConstraint
    {
        public ConstraintType Type { get; set; }
        public Dictionary<string, object> Config { get; set; }
    }
}
