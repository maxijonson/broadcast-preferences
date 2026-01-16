using BroadcastPreferencesPlugin.Entities;
using BroadcastPreferencesPlugin.Configuration;

namespace BroadcastPreferencesPlugin.Plugin;

public partial class BroadcastPreferences
{
    private PluginConfig _config;

    protected override void LoadDefaultConfig()
    {
        _config = new PluginConfig();
        _config.Version = Version;
        _config.Topics = new List<Topic>
        {
            new Topic
            {
                ID = "raidable_bases",
                DisplayName = "Raidable Bases",
                Description = "Raidable Bases notifications",
                Enabled = true,
                SubscribeByDefault = true,
                MessageRegexStrings = new List<string>
                    {
                       @"raidable base event has opened",
                       @"Destroyed a left over raid base at",
                       @"Next automated raid in"
                    },
            },
            new Topic
            {
                ID = "death_notes",
                DisplayName = "Death Notes",
                Description = "Player death notifications",
                Enabled = true,
                SubscribeByDefault = true,
                IgnoreStyles = true,
                MessageRegexStrings = new List<string>
                    {
                        @"^\[DeathNotes\]"
                    },
            },
            new Topic
            {
                ID = "inbound",
                DisplayName = "Inbound Events",
                Description = "Heli, Cargo, Drops and other inbound events",
                Enabled = true,
                SubscribeByDefault = true,
                IgnoreStyles = true,
                MessageRegexStrings = new List<string>
                    {
                        @"^Patrol Helicopter inbound",
                        @"^Cargo Ship inbound",
                        @"^Cargo Ship is approaching the harbor",
                        @"^Cargo Ship has docked at the harbor",
                        @"^Chinook inbound",
                        @"^Bradley APC inbound",
                        @"^Travelling Vendor inbound",
                        @"^Hackable Crate has spawned",
                        @"Cargo Plane inbound",
                        @" has activated The Excavator",
                        @" has requested a supply drop",
                        @" is hacking a locked crate",
                        @" has deployed a supply signal",
                        @"Supply Drop has (dropped|landed)"
                    },
            }
        };
        SaveConfig();
    }

    protected override void LoadConfig()
    {
        base.LoadConfig();
        _config = base.Config.ReadObject<PluginConfig>();
        _config.Version = Version;
        ValidateConfig();
        SaveConfig();
    }

    protected override void SaveConfig() => base.Config.WriteObject(_config);

    private void ValidateConfig()
    {
        // Check if topics have duplicate IDs or names
        var topicIds = new HashSet<string>();
        var topicNames = new HashSet<string>();
        foreach (var topic in _config.Topics)
        {
            if (topicIds.Contains(topic.ID))
            {
                LogWarning($"Duplicate topic ID found in config: '{topic.ID}'. Only the first occurence will be used.");
            }
            else
            {
                topicIds.Add(topic.ID);
            }

            if (topicNames.Contains(topic.DisplayName))
            {
                LogWarning($"Duplicate topic name found in config: '{topic.DisplayName}'. Only the first occurence will match when users refer to topics by name.");
            }
            else
            {
                topicNames.Add(topic.DisplayName);
            }
        }
    }
}