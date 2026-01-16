using BroadcastPreferencesPlugin.Entities;

namespace BroadcastPreferencesPlugin.Plugin;

public partial class BroadcastPreferences
{
    const string CMD_TOPIC_TOGGLE_USAGE = $"/{CMD_TOPIC} toggle <topic>";
    private void HandleTopicToggleCommand(BasePlayer player, string topicName)
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
            subscription.OptedIn = !subscription.OptedIn;
        }
        else
        {
            subscription = new Subscription() { OptedIn = !topic.SubscribeByDefault };
            topicSubs[player.userID] = subscription;
        }

        SaveData();
        player.ChatMessage(
            subscription.OptedIn
                ? m("Topic.Subscribed", player.UserIDString, topic.DisplayName)
                : m("Topic.Unsubscribed", player.UserIDString, topic.DisplayName)
        );
    }
}