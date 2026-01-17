//Define:FileOrder=1400
using Newtonsoft.Json;

namespace BroadcastPreferencesPlugin.Entities;

public class Subscription
{
    [JsonProperty(PropertyName = "Opted In")]
    public bool OptedIn = true;
}