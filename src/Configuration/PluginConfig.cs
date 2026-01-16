using BroadcastPreferencesPlugin.Entities;
using Newtonsoft.Json;
using Oxide.Core;

namespace BroadcastPreferencesPlugin.Configuration;

public class PluginConfig
{
    [JsonProperty(PropertyName = "Topics", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public List<Topic> Topics = new();

    [JsonProperty(PropertyName = "Version")]
    public VersionNumber Version { get; set; }

    public Topic FindTopic(string search)
    {
        var topicByName = Topics.Find(t => string.Equals(t.DisplayName, search, StringComparison.OrdinalIgnoreCase));
        if (topicByName != null)
        {
            return topicByName;
        }
        var topicById = Topics.Find(t => string.Equals(t.ID, search, StringComparison.OrdinalIgnoreCase));
        if (topicById != null)
        {
            return topicById;
        }
        if (int.TryParse(search, out int index) && index > 0 && index <= Topics.Count)
        {
            return Topics[index - 1];
        }
        return null;
    }
}
