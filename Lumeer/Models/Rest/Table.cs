using Lumeer.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lumeer.Models.Rest
{
    public class Table
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public object Priority { get; set; }
        public object Permissions { get; set; }
        public List<TableAttribute> Attributes { get; set; }
        public object Rules{ get; set; }
        public object DataDescription { get; set; }
        public object Purpose { get; set; }
        public string Id { get; set; }
        public int Version { get; set; }
        public bool NonRemovable { get; set; }
        public int DocumentsCount { get; set; }
        public long LastTimeUsed { get; set; }
        public string DefaultAttributeId { get; set; }
        public bool Favorite { get; set; }
        public PurposeType PurposeType{ get; set; }
        public object PurposeMetaData{ get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
