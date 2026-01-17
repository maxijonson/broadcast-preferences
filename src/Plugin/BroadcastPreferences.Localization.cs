//Define:FileOrder=1600
namespace BroadcastPreferencesPlugin.Plugin;

public partial class BroadcastPreferences
{
    private new void LoadDefaultMessages()
    {
        lang.RegisterMessages(
            new Dictionary<string, string>
            {
                ["TestMessage"] = "This is a <color=#ff00ff>test message</color> for <color=yellow>Broadcast Preferences</color> <color=#f0f>plugin</color>.",
                ["Usage"] = "Usage:",
                ["Usage.Description"] = "Listen or mute broadcasted messages by topic.",
                ["Usage.Command"] = "<color=#f0761f>{0}</color> - {1}",
                ["Usage.List"] = "Lists available topics",
                ["Usage.Subscribe"] = "Subscribes to a topic by name or list number",
                ["Usage.Unsubscribe"] = "Unsubscribes from a topic by name or list number",
                ["Usage.Toggle"] = "Toggles subscription to a topic by name or list number",
                ["List.AvailableTopics"] = "Broadcast Preferences (<color=#bcc2c2>Unsubscribed</color> / <color=#62ed53>Subscribed</color>):",
                ["Topic.NotFound"] = "Topic '{0}' not found.",
                ["Topic.AlreadySubscribed"] = "You are already subscribed to '{0}'.",
                ["Topic.NotSubscribed"] = "You are not subscribed to '{0}'.",
                ["Topic.Subscribed"] = "Subscribed to <color=#62ed53>{0}</color>.",
                ["Topic.Unsubscribed"] = "Unsubscribed from <color=#bcc2c2>{0}</color>.",
                ["Topic.ListItemSubscribed"] = "{0}. <color=#62ed53>{1}</color>{2}",
                ["Topic.ListItemUnsubscribed"] = "{0}. <color=#bcc2c2>{1}</color>{2}",
            },
            this,
            "en"
        );
    }
    private string m(string key, string userId = null, params object[] args)
    {
        // Puts($"m({key}, {userId}{(args.Length > 0 ? ", " : "")}{string.Join(", ", args)}) ({args.Length} args)");
        return string.Format(lang.GetMessage(key, this, userId), args);
    }
}