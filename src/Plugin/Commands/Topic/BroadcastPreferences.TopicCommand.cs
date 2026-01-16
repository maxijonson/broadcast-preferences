using Oxide.Plugins;

namespace BroadcastPreferencesPlugin.Plugin;

public partial class BroadcastPreferences
{
    private const string CMD_TOPIC = "topic";
    [ChatCommand(CMD_TOPIC)]
    private void ChatCmdTopic(BasePlayer player, string command, string[] args)
    {
        if (args.Length == 0)
        {
            SendTopicUsageMessage(player);
            return;
        }
        var subCommand = args[0].ToLower();

        switch (subCommand)
        {
            case "list":
                HandleTopicListCommand(player);
                break;
            case "subscribe":
            case "sub":
            case "enable":
            case "on":
                if (args.Length < 2)
                {
                    player.ChatMessage($"{m("Usage", player.UserIDString)} {CMD_TOPIC_SUBSCRIBE_USAGE}");
                    return;
                }
                HandleTopicSubscribeCommand(player, string.Join(" ", args, 1, args.Length - 1));
                break;
            case "unsubscribe":
            case "unsub":
            case "disable":
            case "off":
                if (args.Length < 2)
                {
                    player.ChatMessage($"{m("Usage", player.UserIDString)} {CMD_TOPIC_UNSUBSCRIBE_USAGE}");
                    return;
                }
                HandleTopicUnsubscribeCommand(player, string.Join(" ", args, 1, args.Length - 1));
                break;
            case "toggle":
                if (args.Length < 2)
                {
                    player.ChatMessage($"{m("Usage", player.UserIDString)} {CMD_TOPIC_TOGGLE_USAGE}");
                    return;
                }
                HandleTopicToggleCommand(player, string.Join(" ", args, 1, args.Length - 1));
                break;
            default:
                SendTopicUsageMessage(player);
                break;
        }
    }
}