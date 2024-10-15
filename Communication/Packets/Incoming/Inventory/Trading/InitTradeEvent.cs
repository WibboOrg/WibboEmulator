namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Trading;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Roleplays.Troc;
using WibboEmulator.Games.Rooms;

internal sealed class InitTradeEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!RoomManager.TryGetRoom(session.User.RoomId, out var room))
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
                roomUser.SendWhisperChat(LanguageManager.TryGetValue("rp.troc.zonesafe", session.Language));
                return;
            }

            var rpTarget = roomUserTarget.Roleplayer;
            if (rpTarget == null || rpTarget.TradeId > 0 || rpTarget.Dead || rpTarget.SendPrison || (rpTarget.PvpEnable && room.RoomRoleplay.Pvp) || rpTarget.AggroTimer > 0)
            {
                roomUser.SendWhisperChat(LanguageManager.TryGetValue("rp.troc.fail", session.Language));
                return;
            }

            RPTrocManager.AddTrade(room.RoomData.OwnerId, roomUser.UserId, roomUserTarget.UserId, roomUser.Username, roomUserTarget.Username);
            return;
        }

        if (room.RoomData.TrocStatus == 0 && !room.CheckRights(session, true) && !session.User.HasPermission("force_trade"))
        {
            session.SendNotification(LanguageManager.TryGetValue("notif.trade.error.1", session.Language));
            return;
        }
        else if (room.RoomData.TrocStatus == 1 && !room.CheckRights(session) && !session.User.HasPermission("force_trade"))
        {
            session.SendNotification(LanguageManager.TryGetValue("notif.trade.error.2", session.Language));
            return;
        }

        if (!roomUserTarget.Client.User.AcceptTrading && !session.User.HasPermission("force_trade"))
        {
            session.SendNotification(LanguageManager.TryGetValue("user.tradedisabled", session.Language));
        }
        else if (roomUserTarget.IsTransf || roomUser.IsTransf || roomUser.IsSpectator || roomUserTarget.IsSpectator)
        {
            session.SendNotification(LanguageManager.TryGetValue("notif.trade.error.3", session.Language));
        }
        else
        {
            room.TryStartTrade(roomUser, roomUserTarget);
        }
    }
}
