//Define:FileOrder=200
using System;
using System.Collections.Generic;
using ConVar;
using Network;

namespace BroadcastPreferencesPlugin.Plugin;

public partial class BroadcastPreferences
{
    private object? OnSendCommand(Connection? cn, string? command, object[]? args)
    {
        if (cn == null)
            return null;
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

    private object? OnSendCommand(List<Connection>? connections, string? command, object[]? args)
    {
        if (command != "chat.add" && command != "chat.add2")
            return null;
        if (connections == null || connections.Count == 0)
            return null;

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
            foreach (var cn in connections)
            {
                var player = BasePlayer.FindByID(cn.userid);
                if (player == null || !player.IsConnected)
                    continue;
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
