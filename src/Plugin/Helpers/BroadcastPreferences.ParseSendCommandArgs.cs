//Define:FileOrder=1000
namespace BroadcastPreferencesPlugin.Plugin;

public partial class BroadcastPreferences
{
    private (int channel, ulong userId, string message, string chatIdentifier) ParseSendCommandArgs(object[] args)
    {
        var (channel, userId, message) = ParseBroadcastArgs(args);
        string chatIdentifier = null;

        if (args != null && args.Length >= 4 && args[3] != null)
        {
            chatIdentifier = args[3].ToString();
        }

        return (channel, userId, message, chatIdentifier);
    }
}