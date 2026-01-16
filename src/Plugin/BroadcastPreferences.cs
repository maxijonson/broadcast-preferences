using Oxide.Plugins;

// TODO: Make the command configurable (default: "topic")
// TODO: Admin commands to list subscriptions of a topic or a user
// TODO: Admin commands to clear subscriptions of a topic or a user or all
// TODO: Admin commands to change a user's subscription status for a topic
// TODO: Allow players to create custom topics (with permission control and limits)
// TODO: Add a way to limit what topics a user has control over using permissions:
//      TODO: Add permission to allow the use of the /topic command
//      TODO: Add permission to allow managing a specific topic's subscription status (sub, unsub, toggle for <topic>)
//      TODO: Add topic config option 'Permission Required to Receive'
//      TODO: Add topic config option 'Permission Required to Not Receive'

namespace BroadcastPreferencesPlugin.Plugin;

[Info("Broadcast Preferences", "MaxiJonson", "0.2.0")]
[Description("Allow players to subscribe or unsubscribe from broadcasted messages by topic.")]
public partial class BroadcastPreferences : RustPlugin
{
}