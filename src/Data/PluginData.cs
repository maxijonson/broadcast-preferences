//Define:FileOrder=1500
using BroadcastPreferencesPlugin.Entities;
using Newtonsoft.Json;

namespace BroadcastPreferencesPlugin.Data;

public class PluginData
{
    [JsonProperty(PropertyName = "Subscriptions", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public Dictionary<string, Dictionary<ulong, Subscription>> Subscriptions = new();

    public bool IsPlayerSubscribed(string topicId, ulong userId, bool defaultSubscribe)
    {
        if (Subscriptions.TryGetValue(topicId, out var topicSubs))
        {
            if (topicSubs.TryGetValue(userId, out var subscription))
            {
                return subscription.OptedIn;
            }
        }
        return defaultSubscribe;
    }

    public Dictionary<ulong, Subscription> GetTopicSubscriptions(string topicId)
    {
        if (Subscriptions.TryGetValue(topicId, out var topicSubs))
        {
            return topicSubs;
        }
        Subscriptions[topicId] = new Dictionary<ulong, Subscription>();
        return Subscriptions[topicId];
    }
}

