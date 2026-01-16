using BroadcastPreferencesPlugin.Entities;
using System.Linq;

namespace BroadcastPreferencesPlugin.Plugin;

public partial class BroadcastPreferences
{
    private List<Topic> ResolveBroadcastTopics(ulong userId, string message)
    {
        var matchedTopics = new List<Topic>();
        string stylelessMessage = null; // Don't compute unless needed

        foreach (var topic in _config.Topics)
        {
            if (!topic.Enabled)
                continue;
            if (topic.UserId != 0 && topic.UserId != userId)
                continue;
            if (topic.MessageRegexes != null && topic.MessageRegexes.Count > 0)
            {
                if (topic.IgnoreStyles)
                {
                    if (stylelessMessage == null)
                    {
                        stylelessMessage = GetStylelessMessage(message);
                    }
                    if (!topic.MessageRegexes.Any(regex => regex.IsMatch(stylelessMessage)))
                    {
                        continue;
                    }
                }
                else if (!topic.MessageRegexes.Any(regex => regex.IsMatch(message)))
                {
                    continue;
                }
            }

            matchedTopics.Add(topic);
            if (topic.StopOnMatch)
                break;
        }
        return matchedTopics;
    }
}