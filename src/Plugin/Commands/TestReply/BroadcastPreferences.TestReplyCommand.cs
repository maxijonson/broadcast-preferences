using Oxide.Plugins;

namespace BroadcastPreferencesPlugin.Plugin;

public partial class BroadcastPreferences
{
    [ConsoleCommand("broadcastpreferences.testreply")]
    private void CCmdTestReply(ConsoleSystem.Arg arg)
    {
        if (!arg.IsAdmin) return;
        var message = (arg.Args?.Length ?? 0) > 0 ? string.Join(" ", arg.Args) : m("TestMessage", null, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9);
        foreach (var player in BasePlayer.activePlayerList)
        {
            Player.Reply(player, message, 0L);
        }
    }
}