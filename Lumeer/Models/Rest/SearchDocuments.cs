using Lumeer.Models.Rest.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lumeer.Models.Rest
{
    public class SearchDocuments
    {
        public object Size { get; set; }
        public object ExpandedIds { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public TaskTableAttribute? GroupBy { get; set; }
        public object SortBy { get; set; }
    }
}
