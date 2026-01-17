//Define:FileOrder=200
using ConVar;

namespace BroadcastPreferencesPlugin.Plugin;

public partial class BroadcastPreferences
{
    private object OnBroadcastCommand(string command, object[] args)
    {
        if (command != "chat.add" && command != "chat.add2") return null;

        var (channel, userId, message) = ParseBroadcastArgs(args);
        if (channel != ((int)Chat.ChatChannel.Server))
        {
            // Only handle server broadcasts
            return null;
        }

        if (message == null || message == "") return null;

        if (DispatchMessage(channel, userId, message, BasePlayer.activePlayerList))
        {
            return true;
        }
        return null;
    }
}