using System.Collections.Generic;

namespace Lumeer.Models.Rest
{
    public class NewTask
    {
        public string CollectionId { get; set; }
        public Dictionary<string, object> Data { get; set; }

        public NewTask(string collectionId, Dictionary<string, object> data)
        {
            CollectionId = collectionId;
            Data = data;
        }
    }
}
