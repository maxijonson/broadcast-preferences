using ConVar;
using Network;
using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

//BroadcastPreferences created with PluginMerge v(1.0.14.0) by MJSU @ https://github.com/dassjosh/Plugin.Merge
namespace Oxide.Plugins;

[Info("Broadcast Preferences", "MaxiJonson", "0.2.0")]
[Description("Allow players to subscribe or unsubscribe from broadcasted messages by topic.")]
public partial class BroadcastPreferences : RustPlugin
{
    #region Plugin\BroadcastPreferences.Configuration.cs
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
    #endregion

    #region Plugin\BroadcastPreferences.Data.cs
    private PluginData _data;
    private DynamicConfigFile _dataFile;
    
    private void LoadData()
    {
        _dataFile = Interface.Oxide.DataFileSystem.GetFile(Name);
        _dataFile.Settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        try
        {
            _data = _dataFile.ReadObject<PluginData>();
        }
        catch
        {
            Puts("Could not load data, creating new datafile.");
            _data = new PluginData();
        }
        _dataFile.Clear();
        SaveData();
    }
    
    private void SaveData()
    {
        if (_dataFile != null) _dataFile.WriteObject(_data);
    }
    #endregion

    #region Plugin\BroadcastPreferences.Localization.cs
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
        return string.Format(lang.GetMessage(key, this, userId), args);
    }
    #endregion

    #region Plugin\BroadcastPreferences.Logging.cs
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
    #endregion

    #region Plugin\BroadcastPreferences.Vars.cs
    private new const string Name = "BroadcastPreferences";
    private const string ChatIdentifier = "BroadcastPreferences";
    #endregion

    #region Plugin\Helpers\BroadcastPreferences.DispatchMessage.cs
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
    #endregion

    #region Plugin\Helpers\BroadcastPreferences.GetStylelessMessage.cs
    private string GetStylelessMessage(string message)
    {
        return Regex.Replace(message, @"<color=[^=<\/>]+>|<\/color>|<size=[^=<\/>]+>|<\/size>", "");
    }
    #endregion

    #region Plugin\Helpers\BroadcastPreferences.ParseBroadcastArgs.cs
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
    #endregion

    #region Plugin\Helpers\BroadcastPreferences.ParseSendCommandArgs.cs
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
    #endregion

    #region Plugin\Helpers\BroadcastPreferences.ResolveBroadcastTopics.cs
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
    #endregion

    #region Plugin\Helpers\BroadcastPreferences.SendTopicUsageMessage.cs
    private void SendTopicUsageMessage(BasePlayer player)
    {
        var lines = new[]
        {
            m("Usage.Description", player.UserIDString),
            m("Usage", player.UserIDString),
            m("Usage.Command", player.UserIDString, CMD_TOPIC_LIST_USAGE, m("Usage.List", player.UserIDString)),
            m("Usage.Command", player.UserIDString, CMD_TOPIC_SUBSCRIBE_USAGE, m("Usage.Subscribe", player.UserIDString)),
            m("Usage.Command", player.UserIDString, CMD_TOPIC_UNSUBSCRIBE_USAGE, m("Usage.Unsubscribe", player.UserIDString)),
            m("Usage.Command", player.UserIDString, CMD_TOPIC_TOGGLE_USAGE, m("Usage.Toggle", player.UserIDString)),
        };
        player.ChatMessage(string.Join("\n", lines));
    }
    #endregion

    #region Plugin\Hooks\BroadcastPreferences.Loaded.cs
    private void Loaded()
    {
        LoadData();
    }
    #endregion

    #region Plugin\Hooks\BroadcastPreferences.OnBroadcastCommand.cs
    private object OnBroadcastCommand(string command, object[] args)
    {
        if (command != "chat.add" && command != "chat.add2") return null;
        
        var (channel, userId, message) = ParseBroadcastArgs(args);
        if (channel != ((int)Chat.ChatChannel.Server))
        {
            return null;
        }
        
        if (message == null || message == "") return null;
        
        if (DispatchMessage(channel, userId, message, BasePlayer.activePlayerList))
        {
            return true;
        }
        return null;
    }
    #endregion

    #region Plugin\Hooks\BroadcastPreferences.OnSendCommand.cs
    private object OnSendCommand(Connection cn, string command, object[] args)
    {
        var connections = Facepunch.Pool.Get<List<Connection>>();
        try
        {
            connections.Add(cn);
            return OnSendCommand(connections, command, args);
        }
        catch (Exception ex)
        {
            LogError(ex);
            return null;
        }
        finally
        {
            Facepunch.Pool.FreeUnmanaged(ref connections);
        }
    }
    
    private object OnSendCommand(List<Connection> cn, string command, object[] args)
    {
        if (command != "chat.add" && command != "chat.add2") return null;
        
        var (channel, userId, message, chatIdentifier) = ParseSendCommandArgs(args);
        if (channel != ((int)Chat.ChatChannel.Server))
        {
            return null;
        }
        if (chatIdentifier != null && chatIdentifier == ChatIdentifier)
        {
            return null;
        }
        
        var players = Facepunch.Pool.Get<List<BasePlayer>>();
        var dispatched = false;
        try
        {
            foreach (var connection in cn)
            {
                var player = BasePlayer.FindByID(connection.userid);
                if (player == null || !player.IsConnected) continue;
                players.Add(player);
            }
            dispatched = DispatchMessage(channel, userId, message, players);
        }
        finally
        {
            Facepunch.Pool.FreeUnmanaged(ref players);
        }
        
        return dispatched ? true : null;
    }
    #endregion

    #region Plugin\Commands\TestBroadcast\BroadcastPreferences.TestBroadcastCommand.cs
    [ConsoleCommand("broadcastpreferences.testbroadcast")]
    private void CCmdTest(ConsoleSystem.Arg arg)
    {
        if (!arg.IsAdmin) return;
        var message = (arg.Args?.Length ?? 0) > 0 ? string.Join(" ", arg.Args) : m("TestMessage", null, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9);
        Server.Broadcast(message, 0L);
    }
    #endregion

    #region Plugin\Commands\TestReply\BroadcastPreferences.TestReplyCommand.cs
    [ConsoleCommand("broadcastpreferences.testreply")]
    private void CCmdTestReply(ConsoleSystem.Arg arg)
    {
        if (!arg.IsAdmin) return;
        var message = (arg.Args?.Length ?? 0) > 0 ? string.Join(" ", arg.Args) : m("TestMessage", null, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9);
        foreach (var player in BasePlayer.activePlayerList)
        {
            Player.Reply(player, message, 0L);
        }
    }
    #endregion

    #region Plugin\Commands\Topic\BroadcastPreferences.TopicCommand.cs
    private const string CMD_TOPIC = "topic";
    [ChatCommand(CMD_TOPIC)]
    private void ChatCmdTopic(BasePlayer player, string command, string[] args)
    {
        if (args.Length == 0)
        {
            SendTopicUsageMessage(player);
            return;
        }
        var subCommand = args[0].ToLower();
        
        switch (subCommand)
        {
            case "list":
            HandleTopicListCommand(player);
            break;
            case "subscribe":
            case "sub":
            case "enable":
            case "on":
            if (args.Length < 2)
            {
                player.ChatMessage($"{m("Usage", player.UserIDString)} {CMD_TOPIC_SUBSCRIBE_USAGE}");
                return;
            }
            HandleTopicSubscribeCommand(player, string.Join(" ", args, 1, args.Length - 1));
            break;
            case "unsubscribe":
            case "unsub":
            case "disable":
            case "off":
            if (args.Length < 2)
            {
                player.ChatMessage($"{m("Usage", player.UserIDString)} {CMD_TOPIC_UNSUBSCRIBE_USAGE}");
                return;
            }
            HandleTopicUnsubscribeCommand(player, string.Join(" ", args, 1, args.Length - 1));
            break;
            case "toggle":
            if (args.Length < 2)
            {
                player.ChatMessage($"{m("Usage", player.UserIDString)} {CMD_TOPIC_TOGGLE_USAGE}");
                return;
            }
            HandleTopicToggleCommand(player, string.Join(" ", args, 1, args.Length - 1));
            break;
            default:
            SendTopicUsageMessage(player);
            break;
        }
    }
    #endregion

    #region Plugin\Commands\Topic\BroadcastPreferences.TopicListCommand.cs
    const string CMD_TOPIC_LIST_USAGE = $"/{CMD_TOPIC} list";
    private void HandleTopicListCommand(BasePlayer player)
    {
        var lines = new List<string>
        {
            m("List.AvailableTopics", player.UserIDString)
        };
        for (var i = 0; i < _config.Topics.Count; i++)
        {
            var topic = _config.Topics[i];
            var isSubscribed = _data.IsPlayerSubscribed(topic.ID, player.userID, topic.SubscribeByDefault);
            var description = string.IsNullOrEmpty(topic.Description) ? "" : $" - {topic.Description}";
            lines.Add(m(isSubscribed ? "Topic.ListItemSubscribed" : "Topic.ListItemUnsubscribed", player.UserIDString, i + 1, topic.DisplayName, description));
        }
        player.ChatMessage(string.Join("\n", lines));
    }
    #endregion

    #region Plugin\Commands\Topic\BroadcastPreferences.TopicSubscribeCommand.cs
    const string CMD_TOPIC_SUBSCRIBE_USAGE = $"/{CMD_TOPIC} subscribe <topic>";
    private void HandleTopicSubscribeCommand(BasePlayer player, string topicName)
    {
        var topic = _config.FindTopic(topicName);
        if (topic == null)
        {
            player.ChatMessage(m("Topic.NotFound", player.UserIDString, topicName));
            HandleTopicListCommand(player);
            return;
        }
        
        var topicSubs = _data.GetTopicSubscriptions(topic.ID);
        if (topicSubs.TryGetValue(player.userID, out var subscription))
        {
            if (subscription.OptedIn)
            {
                player.ChatMessage(m("Topic.AlreadySubscribed", player.UserIDString, topic.DisplayName));
                return;
            }
            subscription.OptedIn = true;
        }
        else
        {
            topicSubs[player.userID] = new Subscription { OptedIn = true };
        }
        
        SaveData();
        player.ChatMessage(m("Topic.Subscribed", player.UserIDString, topic.DisplayName));
    }
    #endregion

    #region Plugin\Commands\Topic\BroadcastPreferences.TopicToggleCommand.cs
    const string CMD_TOPIC_TOGGLE_USAGE = $"/{CMD_TOPIC} toggle <topic>";
    private void HandleTopicToggleCommand(BasePlayer player, string topicName)
    {
        var topic = _config.FindTopic(topicName);
        if (topic == null)
        {
            player.ChatMessage(m("Topic.NotFound", player.UserIDString, topicName));
            HandleTopicListCommand(player);
            return;
        }
        
        var topicSubs = _data.GetTopicSubscriptions(topic.ID);
        if (topicSubs.TryGetValue(player.userID, out var subscription))
        {
            subscription.OptedIn = !subscription.OptedIn;
        }
        else
        {
            subscription = new Subscription() { OptedIn = !topic.SubscribeByDefault };
            topicSubs[player.userID] = subscription;
        }
        
        SaveData();
        player.ChatMessage(
        subscription.OptedIn
        ? m("Topic.Subscribed", player.UserIDString, topic.DisplayName)
        : m("Topic.Unsubscribed", player.UserIDString, topic.DisplayName)
        );
    }
    #endregion

    #region Plugin\Commands\Topic\BroadcastPreferences.TopicUnsubscribeCommand.cs
    const string CMD_TOPIC_UNSUBSCRIBE_USAGE = $"/{CMD_TOPIC} unsubscribe <topic>";
    private void HandleTopicUnsubscribeCommand(BasePlayer player, string topicName)
    {
        var topic = _config.FindTopic(topicName);
        if (topic == null)
        {
            player.ChatMessage(m("Topic.NotFound", player.UserIDString, topicName));
            HandleTopicListCommand(player);
            return;
        }
        
        var topicSubs = _data.GetTopicSubscriptions(topic.ID);
        if (topicSubs.TryGetValue(player.userID, out var subscription))
        {
            if (!subscription.OptedIn)
            {
                player.ChatMessage(m("Topic.NotSubscribed", player.UserIDString, topic.DisplayName));
                return;
            }
            subscription.OptedIn = false;
        }
        else
        {
            topicSubs[player.userID] = new Subscription { OptedIn = false };
        }
        
        SaveData();
        player.ChatMessage(m("Topic.Unsubscribed", player.UserIDString, topic.DisplayName));
    }
    #endregion

    #region Configuration\PluginConfig.cs
    public class PluginConfig
    {
        [JsonProperty(PropertyName = "Topics", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<Topic> Topics = new();
        
        [JsonProperty(PropertyName = "Version")]
        public VersionNumber Version { get; set; }
        
        public Topic FindTopic(string search)
        {
            var topicByName = Topics.Find(t => string.Equals(t.DisplayName, search, StringComparison.OrdinalIgnoreCase));
            if (topicByName != null)
            {
                return topicByName;
            }
            var topicById = Topics.Find(t => string.Equals(t.ID, search, StringComparison.OrdinalIgnoreCase));
            if (topicById != null)
            {
                return topicById;
            }
            if (int.TryParse(search, out int index) && index > 0 && index <= Topics.Count)
            {
                return Topics[index - 1];
            }
            return null;
        }
    }
    #endregion

    #region Data\PluginData.cs
    public class PluginData
    {
        [JsonProperty(PropertyName = "Subscriptions", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<string, Dictionary<ulong, Subscription>> Subscriptions = new();
        
        public bool IsPlayerSubscribed(string topicId, ulong userId, bool defaultSubscribe)
        {
            if (Subscriptions.TryGetValue(topicId, out var topicSubs))
            {
                if (topicSubs.TryGetValue(userId, out var subscription))
                {
                    return subscription.OptedIn;
                }
            }
            return defaultSubscribe;
        }
        
        public Dictionary<ulong, Subscription> GetTopicSubscriptions(string topicId)
        {
            if (Subscriptions.TryGetValue(topicId, out var topicSubs))
            {
                return topicSubs;
            }
            Subscriptions[topicId] = new Dictionary<ulong, Subscription>();
            return Subscriptions[topicId];
        }
    }
    #endregion

    #region Entities\Subscription.cs
    public class Subscription
    {
        [JsonProperty(PropertyName = "Opted In")]
        public bool OptedIn = true;
    }
    #endregion

    #region Entities\Topic.cs
    public class Topic
    {
        [JsonProperty(PropertyName = "Topic ID")]
        public string ID = "";
        
        [JsonProperty(PropertyName = "Display Name")]
        public string DisplayName = "";
        
        [JsonProperty(PropertyName = "Enabled")]
        public bool Enabled = true;
        
        [JsonProperty(PropertyName = "Description")]
        public string Description = "";
        
        [JsonProperty(PropertyName = "Subscribe By Default")]
        public bool SubscribeByDefault = true;
        
        [JsonProperty(PropertyName = "User ID Match (0=Ignore)")]
        public ulong UserId = 0;
        
        [JsonProperty(PropertyName = "Message Regex Match (empty=Ignore)")]
        public List<string> MessageRegexStrings = new();
        
        [JsonIgnore]
        public List<Regex> MessageRegexes { get; private set; }
        
        [JsonProperty(PropertyName = "Override Steam Avatar User ID")]
        public ulong SteamAvatarUserID = 0;
        
        [JsonProperty(PropertyName = "Stop On Match")]
        public bool StopOnMatch = false;
        
        [JsonProperty(PropertyName = "Ignore Styles")]
        public bool IgnoreStyles = true;
        
        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            if (string.IsNullOrEmpty(DisplayName))
            {
                DisplayName = string.IsNullOrEmpty(ID) ? Guid.NewGuid().ToString().Substring(0, 8) : ID;
            }
            if (string.IsNullOrEmpty(ID))
            {
                ID = DisplayName.Replace(" ", "_").ToLower();
            }
            if (MessageRegexStrings != null && MessageRegexStrings.Count > 0)
            {
                try
                {
                    MessageRegexes = MessageRegexStrings.Select(pattern => new Regex(pattern, RegexOptions.Compiled)).ToList();
                }
                catch
                {
                    BroadcastPreferences.LogError($"Invalid regex pattern in topic '{DisplayName}': '{MessageRegexStrings}'");
                    MessageRegexes = null;
                }
            }
            else
            {
                MessageRegexes = null;
            }
        }
    }
    #endregion

}

