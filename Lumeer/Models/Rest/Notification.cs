using System.Collections.Generic;

namespace Lumeer.Models.Rest
{
    public class Notification
    {
        public long CreatedAt { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public object FirstReadAt { get; set; }
        public string Id { get; set; }
        public bool Read { get; set; }
        public string Type { get; set; }
        public string UserId { get; set; }
    }
}
