using BroadcastPreferencesPlugin.Entities;

namespace BroadcastPreferencesPlugin.Plugin;

public partial class BroadcastPreferences
{
    const string CMD_TOPIC_UNSUBSCRIBE_USAGE = $"/{CMD_TOPIC} unsubscribe <topic>";
    private void HandleTopicUnsubscribeCommand(BasePlayer player, string topicName)
    {
        var topic = _config.FindTopic(topicName);
        if (topic == null)
        {
            player.ChatMessage(m("Topic.NotFound", player.UserIDString, topicName));
            HandleTopicListCommand(player);
            return;
        }

        var topicSubs = _data.GetTopicSubscriptions(topic.ID);
        if (topicSubs.TryGetValue(player.userID, out var subscription))
        {
            if (!subscription.OptedIn)
            {
                player.ChatMessage(m("Topic.NotSubscribed", player.UserIDString, topic.DisplayName));
                return;
            }
            subscription.OptedIn = false;
        }
        else
        {
            topicSubs[player.userID] = new Subscription { OptedIn = false };
        }

        SaveData();
        player.ChatMessage(m("Topic.Unsubscribed", player.UserIDString, topic.DisplayName));
    }
}