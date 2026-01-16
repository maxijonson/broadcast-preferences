using System.Text.RegularExpressions;

namespace BroadcastPreferencesPlugin.Plugin;

public partial class BroadcastPreferences
{
    private string GetStylelessMessage(string message)
    {
        return Regex.Replace(message, @"<color=[^=<\/>]+>|<\/color>|<size=[^=<\/>]+>|<\/size>", "");
    }
}