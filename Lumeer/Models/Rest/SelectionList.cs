using System.Collections.Generic;

namespace Lumeer.Models.Rest
{
    public class SelectionList
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string OrganizationId { get; set; }
        public string ProjectId { get; set; }
        public bool DisplayValues { get; set; }
        public List<SelectionOption> Options { get; set; }
    }
}
