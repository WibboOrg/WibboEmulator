namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Trading;
using WibboEmulator.Games.GameClients;

internal sealed class InitTradeEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.User.CurrentRoomId, out var room))
        {
            return;
        }

        var virtualId = packet.PopInt();

        var roomUser = room.RoomUserManager.GetRoomUserByUserId(session.User.Id);
        var roomUserTarget = room.RoomUserManager.GetRoomUserByVirtualId(virtualId);
        if (roomUser == null || roomUser.Client == null || roomUser.Client.User == null)
        {
            return;
        }

        if (roomUserTarget == null || roomUserTarget.Client == null || roomUserTarget.Client.User == null)
        {
            return;
        }

        if (room.IsRoleplay)
        {
            var rp = roomUser.Roleplayer;
            if (rp == null || rp.TradeId > 0 || rp.Dead || rp.SendPrison || (rp.PvpEnable && room.RoomRoleplay.Pvp) || rp.AggroTimer > 0)
            {
                roomUser.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("rp.troc.zonesafe", session.Langue));
                return;
            }

            var rpTarget = roomUserTarget.Roleplayer;
            if (rpTarget == null || rpTarget.TradeId > 0 || rpTarget.Dead || rpTarget.SendPrison || (rpTarget.PvpEnable && room.RoomRoleplay.Pvp) || rpTarget.AggroTimer > 0)
            {
                roomUser.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("rp.troc.fail", session.Langue));
                return;
            }

            WibboEnvironment.GetGame().GetRoleplayManager().TrocManager.AddTrade(room.RoomData.OwnerId, roomUser.UserId, roomUserTarget.UserId, roomUser.GetUsername(), roomUserTarget.GetUsername());
            return;
        }

        if (room.RoomData.TrocStatus == 0)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.trade.error.1", session.Langue));
            return;
        }
        else if (room.RoomData.TrocStatus == 1 && !room.CheckRights(session))
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.trade.error.2", session.Langue));
            return;
        }

        if (!roomUserTarget.Client.User.AcceptTrading && session.User.Rank < 3)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("user.tradedisabled", session.Langue));
        }
        else if (roomUserTarget.IsTransf || roomUser.IsTransf || roomUser.IsSpectator || roomUserTarget.IsSpectator)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.trade.error.3", session.Langue));
        }
        else
        {
            room.TryStartTrade(roomUser, roomUserTarget);
        }
    }
}
