//Define:FileOrder=1000
namespace BroadcastPreferencesPlugin.Plugin;

public partial class BroadcastPreferences
{
    private bool DispatchMessage(int channel, ulong userId, string message, IEnumerable<BasePlayer> players)
    {
        var remainingPlayers = Facepunch.Pool.Get<List<BasePlayer>>();
        try
        {
            remainingPlayers.AddRange(players);
            if (remainingPlayers.Count == 0) return false;

            var topics = ResolveBroadcastTopics(userId, message);
            if (topics.Count == 0) return false;

            var sentPlayers = new HashSet<ulong>();
            foreach (var topic in topics)
            {
                if (remainingPlayers.Count == 0) break;

                if (topic.SteamAvatarUserID != 0)
                {
                    userId = topic.SteamAvatarUserID;
                }

                var topicData = _data.Subscriptions.GetValueOrDefault(topic.ID);

                for (int i = remainingPlayers.Count - 1; i >= 0; i--)
                {
                    var player = remainingPlayers[i];
                    if (sentPlayers.Contains(player.userID)) continue;

                    if (topicData != null && topicData.Count > 0)
                    {
                        if (!topicData.ContainsKey(player.userID))
                        {
                            if (!topic.SubscribeByDefault)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            var subscription = topicData[player.userID];
                            if (!subscription.OptedIn)
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        if (!topic.SubscribeByDefault)
                        {
                            continue;
                        }
                    }

                    player.SendConsoleCommand("chat.add", channel, userId, message, ChatIdentifier);
                    sentPlayers.Add(player.userID);
                    remainingPlayers.RemoveAt(i);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            LogError(ex);
        }
        finally
        {
            Facepunch.Pool.FreeUnmanaged(ref remainingPlayers);
        }
        return false;
    }
}