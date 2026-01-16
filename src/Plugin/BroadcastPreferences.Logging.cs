using Oxide.Core;
using System;

namespace BroadcastPreferencesPlugin.Plugin;

public partial class BroadcastPreferences
{
    // Puts doesn't work in all contexts (e.g: classes), so we redefine it here
    protected static void Puts(Exception ex)
    {
        Interface.Oxide.LogInfo("[{0}] {1}", Name, ex);
    }

    protected new static void Puts(string format, params object[] args)
    {
        if (!string.IsNullOrWhiteSpace(format))
        {
            Interface.Oxide.LogInfo("[{0}] {1}", Name, (args.Length != 0) ? string.Format(format, args) : format);
        }
    }
    protected static void LogWarning(Exception ex)
    {
        Interface.Oxide.LogWarning("[{0}] {1}", Name, ex);
    }

    protected static void LogWarning(string format, params object[] args)
    {
        if (!string.IsNullOrWhiteSpace(format))
        {
            Interface.Oxide.LogWarning("[{0}] {1}", Name, (args.Length != 0) ? string.Format(format, args) : format);
        }
    }
    public static void LogError(Exception ex)
    {
        Interface.Oxide.LogError("[{0}] {1}", Name, ex);
    }

    public static void LogError(string format, params object[] args)
    {
        if (!string.IsNullOrWhiteSpace(format))
        {
            Interface.Oxide.LogError("[{0}] {1}", Name, (args.Length != 0) ? string.Format(format, args) : format);
        }
    }
}