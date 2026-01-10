Allows players to subscribe and unsubscribe to broadcast messages sent by other plugins. This helps manage notification fatigue in servers with multiple plugins sending frequent messages.

## Features

- **Personalization**: Each player can subscribe or unsubscribe to specific broadcast topics.
- **Flexible Categorization**: Topics can be defined by server admins to match broadcast messages using regular expressions, allowing for flexible categorization. This also means that this plugin is non-intrusive and works with existing and future plugins without modification.
- **Message Support**: Supports broadcasted (`Server.Broadcast`) and direct (`Player.Reply`) messages sent from other plugins.

## Chat Commands

- `/topic` - Shows usage information for the plugin.
- `/topic list` - Lists all available broadcast topics along with the player's current subscription status.
- `/topic subscribe <topic name|number>` - Subscribes the player to the specified topic.
  - Aliases for `subscribe`: `sub`, `enable`, `on`
- `/topic unsubscribe <topic name|number>` - Unsubscribes the player from the specified topic.
  - Aliases for `unsubscribe`: `unsub`, `disable`, `off`
- `/topic toggle <topic name|number>` - Toggles the player's subscription status for the specified topic.

## Console Commands

- `broadcastpreferences.testbroadcast [message]` - Sends a test broadcast message to the console and all players. If no message is provided, a default test message is used. Can be useful for testing your topic regex configurations.
- `broadcastpreferences.testreply [message]` - Sends a test direct reply message to all players. If no message is provided, a default test message is used. Can be useful for testing your topic regex configurations.

## Configuration

> The settings and options can be configured in the `BroadcastPreferences` file under the `config` directory. The use of an editor and validator is recommended to avoid formatting issues and syntax errors.

```json
{
  "Topics": [
    {
      "Topic ID": "raidable_bases",
      "Display Name": "Raidable Bases",
      "Enabled": true,
      "Description": "Raidable Bases notifications",
      "Subscribe By Default": true,
      "User ID Match (0=Ignore)": 0,
      "Message Regex Match (empty=Ignore)": [
        "raidable base event has opened",
        "Destroyed a left over raid base at",
        "Next automated raid in"
      ],
      "Override Steam Avatar User ID": 0,
      "Stop On Match": false,
      "Ignore Styles": true
    },
    {
      "Topic ID": "death_notes",
      "Display Name": "Death Notes",
      "Enabled": true,
      "Description": "Player death notifications",
      "Subscribe By Default": true,
      "User ID Match (0=Ignore)": 0,
      "Message Regex Match (empty=Ignore)": ["^\\[DeathNotes\\]"],
      "Override Steam Avatar User ID": 0,
      "Stop On Match": false,
      "Ignore Styles": true
    },
    {
      "Topic ID": "inbound",
      "Display Name": "Inbound Events",
      "Enabled": true,
      "Description": "Heli, Cargo, Drops and other inbound events",
      "Subscribe By Default": true,
      "User ID Match (0=Ignore)": 0,
      "Message Regex Match (empty=Ignore)": [
        "^Patrol Helicopter inbound",
        "^Cargo Ship inbound",
        "^Cargo Ship is approaching the harbor",
        "^Cargo Ship has docked at the harbor",
        "^Chinook inbound",
        "^Bradley APC inbound",
        "^Travelling Vendor inbound",
        "^Hackable Crate has spawned",
        "Cargo Plane inbound",
        " has activated The Excavator",
        " has requested a supply drop",
        " is hacking a locked crate",
        " has deployed a supply signal",
        "Supply Drop has (dropped|landed)"
      ],
      "Override Steam Avatar User ID": 0,
      "Stop On Match": false,
      "Ignore Styles": true
    }
  ],
  "Version": {
    "Major": 0,
    "Minor": 1,
    "Patch": 0
  }
}
```

- `Topics`: An array of topic objects defining the broadcast categories. The order of topics only matters if the `Stop On Match` option is used. Topics are evaluated in the order they are listed.
  - `Topic ID`: A unique identifier for the topic.
  - `Display Name`: The name displayed in chat to players for this topic.
  - `Enabled`: Whether the topic is active.
  - `Description`: A brief description of the topic that players will see in the `/topic list` command.
  - `Subscribe By Default`: Default subscription status for players who have not explicitly set their preference.
  - `User ID Match`: If set to a specific user ID, only messages from that user will be considered for this topic. Set to 0 to ignore user ID filtering. This can be useful that allow you to configure the user ID used when sending messages. You can specify a random user ID that will match the topic, then override the avatar using the `Override Steam Avatar User ID` setting.
  - `Message Regex Match`: An array of regular expressions used to match broadcast messages for this topic. You may specify multiple regex patterns for a single topic. The first matching pattern will trigger the topic.
  - `Override Steam Avatar User ID`: If set, this user ID's avatar will be used in notifications for this topic. Particularly useful when combined with the `User ID Match` setting.
  - `Stop On Match`: If true, no further topics will be checked after a match is found for this topic. In practice, you shouldn't really need this option unless you've got two topics with overlapping regex patterns.
    - **Scenario**: 
      - Topic A: `^\\[Important\\]` with `Stop On Match` set to true
      - Topic B: `Important`
      - Message: `[Important] Server will restart in 10 minutes.`
      - Player is **unsubscribed** to Topic A but **subscribed** to Topic B.
      - Result: The message matches Topic A and stops looking for more topics, so the player does not receive the message, even though they are subscribed to Topic B, because Topic A stopped further processing.
  - `Ignore Styles`: If true, the plugin will strip any `<color>` and `<size>` tags from messages before processing them for this topic.

## Localization

> The default messages are in the `BroadcastPreferences` file under the `lang/en` directory. To add support for another language, create a new language folder (e.g. `de` for German) if not already created, copy the default language file to the new folder and then customize the messages.

```json
{
  "TestMessage": "This is a <color=#ff00ff>test message</color> for <color=yellow>Broadcast Subscriptions</color> <color=#f0f>plugin</color>.",
  "Usage": "Usage:",
  "Usage.Description": "Listen or mute broadcasted messages by topic.",
  "Usage.Command": "<color=#f0761f>{0}</color> - {1}",
  "Usage.List": "Lists available topics",
  "Usage.Subscribe": "Subscribes to a topic by name or list number",
  "Usage.Unsubscribe": "Unsubscribes from a topic by name or list number",
  "Usage.Toggle": "Toggles subscription to a topic by name or list number",
  "List.AvailableTopics": "Broadcast Preferences (<color=#bcc2c2>Unsubscribed</color> / <color=#62ed53>Subscribed</color>):",
  "Topic.NotFound": "Topic '{0}' not found.",
  "Topic.AlreadySubscribed": "You are already subscribed to '{0}'.",
  "Topic.NotSubscribed": "You are not subscribed to '{0}'.",
  "Topic.Subscribed": "Subscribed to <color=#62ed53>{0}</color>.",
  "Topic.Unsubscribed": "Unsubscribed from <color=#bcc2c2>{0}</color>.",
  "Topic.ListItemSubscribed": "{0}. <color=#62ed53>{1}</color>{2}",
  "Topic.ListItemUnsubscribed": "{0}. <color=#bcc2c2>{1}</color>{2}"
}
```

## Guidelines

- You can test your regex patterns using online tools like [Regexr](https://regexr.com/).
- Check your regex patterns for ReDoS vulnerabilities using tools like [Devina's ReDoS Checker](https://devina.io/redos-checker). ReDoS vulnerabilities can lead to server performance issues or crashes.
- Remember about localization: you may (or may not) want to add translated versions of your regex patterns for different server languages. The plugin obviously cannot translate english regexes into other languages automatically.
