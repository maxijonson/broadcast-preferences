//Define:FileOrder=1000
namespace BroadcastPreferencesPlugin.Plugin;

public partial class BroadcastPreferences
{
    private (int channel, ulong userId, string message) ParseBroadcastArgs(object[] args)
    {
        int channel = 0;
        ulong userId = 0;
        string message = "";

        if (args != null)
        {
            if (args.Length >= 1)
            {
                int.TryParse(args[0].ToString(), out channel);
            }

            if (args.Length >= 2)
            {
                ulong.TryParse(args[1].ToString(), out userId);
            }

            if (args.Length >= 3 && args[2] != null)
            {
                message = args[2].ToString();
            }
        }

        return (channel, userId, message);
    }
}