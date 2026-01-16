using ConVar;
using Network;
using System.Collections.Generic;

namespace BroadcastPreferencesPlugin.Plugin;

public partial class BroadcastPreferences
{
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
            // Only handle server broadcasts
            return null;
        }
        if (chatIdentifier != null && chatIdentifier == ChatIdentifier)
        {
            // Ignore messages sent by this plugin
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
}