using Lumeer.Models.Rest.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lumeer.Models.Rest
{
    public class SearchConfig
    {
        public string Key { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public PerspectiveType Perspective { get; set; }
        public Config Config { get; set; }
        public long UpdatedAt { get; set; }
        public string UserId { get; set; }

        public SearchConfig(DefaultConfig defaultConfig)
        {
            Key = defaultConfig.Key;
            Perspective = defaultConfig.Perspective;
            Config = JsonConvert.DeserializeObject<Config>(defaultConfig.Config.ToString());
            UpdatedAt = defaultConfig.UpdatedAt;
            UserId = defaultConfig.UserId;
        }
    }
}
