//Define:FileOrder=1400
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using BroadcastPreferencesPlugin.Plugin;
using Newtonsoft.Json;

namespace BroadcastPreferencesPlugin.Entities;

public class Topic
{
    [JsonProperty(PropertyName = "Topic ID")]
    public string ID = "";

    [JsonProperty(PropertyName = "Display Name")]
    public string DisplayName = "";

    [JsonProperty(PropertyName = "Enabled")]
    public bool Enabled = true;

    [JsonProperty(PropertyName = "Description")]
    public string Description = "";

    [JsonProperty(PropertyName = "Subscribe By Default")]
    public bool SubscribeByDefault = true;

    [JsonProperty(PropertyName = "User ID Match (0=Ignore)")]
    public ulong UserId = 0;

    [JsonProperty(PropertyName = "Message Regex Match (empty=Ignore)")]
    public List<string> MessageRegexStrings = new();

    [JsonIgnore]
    public List<Regex> MessageRegexes { get; private set; }

    [JsonProperty(PropertyName = "Override Steam Avatar User ID")]
    public ulong SteamAvatarUserID = 0;

    [JsonProperty(PropertyName = "Stop On Match")]
    public bool StopOnMatch = false;

    [JsonProperty(PropertyName = "Ignore Styles")]
    public bool IgnoreStyles = true;

    [OnDeserialized]
    internal void OnDeserializedMethod(StreamingContext context)
    {
        if (string.IsNullOrEmpty(DisplayName))
        {
            DisplayName = string.IsNullOrEmpty(ID) ? Guid.NewGuid().ToString().Substring(0, 8) : ID;
        }
        if (string.IsNullOrEmpty(ID))
        {
            ID = DisplayName.Replace(" ", "_").ToLower();
        }
        if (MessageRegexStrings != null && MessageRegexStrings.Count > 0)
        {
            try
            {
                MessageRegexes = MessageRegexStrings.Select(pattern => new Regex(pattern, RegexOptions.Compiled)).ToList();
            }
            catch
            {
                BroadcastPreferences.LogError($"Invalid regex pattern in topic '{DisplayName}': '{MessageRegexStrings}'");
                MessageRegexes = null;
            }
        }
        else
        {
            MessageRegexes = null;
        }
    }
}