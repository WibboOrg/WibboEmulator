namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Trading;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Roleplays.Troc;
using WibboEmulator.Games.Rooms;

internal sealed class InitTradeEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (!RoomManager.TryGetRoom(Session.User.RoomId, out var room))
        {
            return;
        }

        var virtualId = packet.PopInt();

        var roomUser = room.RoomUserManager.GetRoomUserByUserId(Session.User.Id);
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
                roomUser.SendWhisperChat(LanguageManager.TryGetValue("rp.troc.zonesafe", Session.Language));
                return;
            }

            var rpTarget = roomUserTarget.Roleplayer;
            if (rpTarget == null || rpTarget.TradeId > 0 || rpTarget.Dead || rpTarget.SendPrison || (rpTarget.PvpEnable && room.RoomRoleplay.Pvp) || rpTarget.AggroTimer > 0)
            {
                roomUser.SendWhisperChat(LanguageManager.TryGetValue("rp.troc.fail", Session.Language));
                return;
            }

            RPTrocManager.AddTrade(room.RoomData.OwnerId, roomUser.UserId, roomUserTarget.UserId, roomUser.Username, roomUserTarget.Username);
            return;
        }

        if (room.RoomData.TrocStatus == 0 && !room.CheckRights(Session, true) && !Session.User.HasPermission("force_trade"))
        {
            Session.SendNotification(LanguageManager.TryGetValue("notif.trade.error.1", Session.Language));
            return;
        }
        else if (room.RoomData.TrocStatus == 1 && !room.CheckRights(Session) && !Session.User.HasPermission("force_trade"))
        {
            Session.SendNotification(LanguageManager.TryGetValue("notif.trade.error.2", Session.Language));
            return;
        }

        if (!roomUserTarget.Client.User.AcceptTrading && !Session.User.HasPermission("force_trade"))
        {
            Session.SendNotification(LanguageManager.TryGetValue("user.tradedisabled", Session.Language));
        }
        else if (roomUserTarget.IsTransf || roomUser.IsTransf || roomUser.IsSpectator || roomUserTarget.IsSpectator)
        {
            Session.SendNotification(LanguageManager.TryGetValue("notif.trade.error.3", Session.Language));
        }
        else
        {
            room.TryStartTrade(roomUser, roomUserTarget);
        }
    }
}
