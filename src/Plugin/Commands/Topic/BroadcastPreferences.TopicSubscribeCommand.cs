//Define:FileOrder=311
using BroadcastPreferencesPlugin.Entities;

namespace BroadcastPreferencesPlugin.Plugin;

public partial class BroadcastPreferences
{
    const string CMD_TOPIC_SUBSCRIBE_USAGE = $"/{CMD_TOPIC} subscribe <topic>";
    private void HandleTopicSubscribeCommand(BasePlayer player, string topicName)
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
            if (subscription.OptedIn)
            {
                player.ChatMessage(m("Topic.AlreadySubscribed", player.UserIDString, topic.DisplayName));
                return;
            }
            subscription.OptedIn = true;
        }
        else
        {
            topicSubs[player.userID] = new Subscription { OptedIn = true };
        }

        SaveData();
        player.ChatMessage(m("Topic.Subscribed", player.UserIDString, topic.DisplayName));
    }
}