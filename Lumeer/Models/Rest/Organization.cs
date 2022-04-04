namespace Lumeer.Models.Rest
{
    public class Organization
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public object Priority { get; set; }
        public object Permissions { get; set; }
        public string Id { get; set; }
        public int Version { get; set; }
        public bool NonRemovable { get; set; }
    }
}
