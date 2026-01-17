//Define:FileOrder=1000
namespace BroadcastPreferencesPlugin.Plugin;

public partial class BroadcastPreferences
{
    private void SendTopicUsageMessage(BasePlayer player)
    {
        var lines = new[]
        {
            m("Usage.Description", player.UserIDString),
            m("Usage", player.UserIDString),
            m("Usage.Command", player.UserIDString, CMD_TOPIC_LIST_USAGE, m("Usage.List", player.UserIDString)),
            m("Usage.Command", player.UserIDString, CMD_TOPIC_SUBSCRIBE_USAGE, m("Usage.Subscribe", player.UserIDString)),
            m("Usage.Command", player.UserIDString, CMD_TOPIC_UNSUBSCRIBE_USAGE, m("Usage.Unsubscribe", player.UserIDString)),
            m("Usage.Command", player.UserIDString, CMD_TOPIC_TOGGLE_USAGE, m("Usage.Toggle", player.UserIDString)),
        };
        player.ChatMessage(string.Join("\n", lines));
    }
}