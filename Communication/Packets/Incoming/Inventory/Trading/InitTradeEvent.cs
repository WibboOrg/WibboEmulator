namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Trading;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class InitTradeEvent : IPacketEvent
{
    public double Delay => 5000;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        if (room.IsRoleplay)
        {
            var RoomUser = room.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
            var RoomUserTarget = room.GetRoomUserManager().GetRoomUserByVirtualId(Packet.PopInt());
            if (RoomUser == null || RoomUser.GetClient() == null || RoomUser.GetClient().GetUser() == null)
            {
                return;
            }

            if (RoomUserTarget == null || RoomUserTarget.GetClient() == null || RoomUserTarget.GetClient().GetUser() == null)
            {
                return;
            }

            var Rp = RoomUser.Roleplayer;
            if (Rp == null || Rp.TradeId > 0 || Rp.Dead || Rp.SendPrison || Rp.PvpEnable && room.Roleplay.Pvp || Rp.AggroTimer > 0)
            {
                RoomUser.SendWhisperChat("Vous devez être en zone safe pour pouvoir troquer");
                return;
            }

            var RpTarget = RoomUserTarget.Roleplayer;
            if (RpTarget == null || RpTarget.TradeId > 0 || RpTarget.Dead || RpTarget.SendPrison || RpTarget.PvpEnable && room.Roleplay.Pvp || RpTarget.AggroTimer > 0)
            {
                RoomUser.SendWhisperChat("Ce joueur ne peut pas troc");
                return;
            }

            WibboEnvironment.GetGame().GetRoleplayManager().GetTrocManager().AddTrade(room.RoomData.OwnerId, RoomUser.UserId, RoomUserTarget.UserId, RoomUser.GetUsername(), RoomUserTarget.GetUsername());
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

        var roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
        var roomUserByVirtualId = room.GetRoomUserManager().GetRoomUserByVirtualId(Packet.PopInt());
        if (roomUserByVirtualId == null || roomUserByVirtualId.GetClient() == null || roomUserByVirtualId.GetClient().GetUser() == null)
        {
            return;
        }

        if (!roomUserByVirtualId.GetClient().GetUser().AcceptTrading && session.GetUser().Rank < 3)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("user.tradedisabled", session.Langue));
        }
        else if (roomUserByVirtualId.IsTransf || roomUserByUserId.IsTransf || roomUserByUserId.IsSpectator || roomUserByVirtualId.IsSpectator)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.trade.error.3", session.Langue));
        }
        else
        {
            room.TryStartTrade(roomUserByUserId, roomUserByVirtualId);
        }
    }
}