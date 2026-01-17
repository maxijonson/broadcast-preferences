//Define:FileOrder=311
namespace BroadcastPreferencesPlugin.Plugin;

public partial class BroadcastPreferences
{
    const string CMD_TOPIC_LIST_USAGE = $"/{CMD_TOPIC} list";
    private void HandleTopicListCommand(BasePlayer player)
    {
        var lines = new List<string>
        {
            m("List.AvailableTopics", player.UserIDString)
        };
        for (var i = 0; i < _config.Topics.Count; i++)
        {
            var topic = _config.Topics[i];
            var isSubscribed = _data.IsPlayerSubscribed(topic.ID, player.userID, topic.SubscribeByDefault);
            var description = string.IsNullOrEmpty(topic.Description) ? "" : $" - {topic.Description}";
            lines.Add(m(isSubscribed ? "Topic.ListItemSubscribed" : "Topic.ListItemUnsubscribed", player.UserIDString, i + 1, topic.DisplayName, description));
        }
        player.ChatMessage(string.Join("\n", lines));
    }
}