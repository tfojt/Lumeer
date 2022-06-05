using Lumeer.Models.Rest.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lumeer.Models.Rest
{
    public class TableAttribute
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public object Description { get; set; }
        public AttributeConstraint Constraint { get; set; }
        public object Lock { get; set; }
        public object Function { get; set; }
        public int UsageCount { get; set; }

        public ConstraintType ConstraintType => Constraint != null ? Constraint.Type : ConstraintType.None;
    }
}
