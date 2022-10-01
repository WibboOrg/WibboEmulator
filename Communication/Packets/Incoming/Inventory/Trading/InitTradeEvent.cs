using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Roleplay.Player;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class InitTradeEvent : IPacketEvent
    {
        public double Delay => 5000;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room room))
                return;

            if (room.IsRoleplay)
            {
                RoomUser RoomUser = room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
                RoomUser RoomUserTarget = room.GetRoomUserManager().GetRoomUserByVirtualId(Packet.PopInt());
                if (RoomUser == null || RoomUser.GetClient() == null || RoomUser.GetClient().GetUser() == null)
                {
                    return;
                }

                if (RoomUserTarget == null || RoomUserTarget.GetClient() == null || RoomUserTarget.GetClient().GetUser() == null)
                {
                    return;
                }

                RolePlayer Rp = RoomUser.Roleplayer;
                if (Rp == null || Rp.TradeId > 0 || Rp.Dead || Rp.SendPrison || (Rp.PvpEnable && room.Roleplay.Pvp) || Rp.AggroTimer > 0)
                {
                    RoomUser.SendWhisperChat("Vous devez être en zone safe pour pouvoir troquer");
                    return;
                }

                RolePlayer RpTarget = RoomUserTarget.Roleplayer;
                if (RpTarget == null || RpTarget.TradeId > 0 || RpTarget.Dead || RpTarget.SendPrison || (RpTarget.PvpEnable && room.Roleplay.Pvp) || RpTarget.AggroTimer > 0)
                {
                    RoomUser.SendWhisperChat("Ce joueur ne peut pas troc");
                    return;
                }

                WibboEnvironment.GetGame().GetRoleplayManager().GetTrocManager().AddTrade(room.RoomData.OwnerId, RoomUser.UserId, RoomUserTarget.UserId, RoomUser.GetUsername(), RoomUserTarget.GetUsername());
                return;
            }

            if (room.RoomData.TrocStatus == 0)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.trade.error.1", Session.Langue));
                return;
            }
            else if (room.RoomData.TrocStatus == 1 && !room.CheckRights(Session))
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.trade.error.2", Session.Langue));
                return;
            }

            RoomUser roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
            RoomUser roomUserByVirtualId = room.GetRoomUserManager().GetRoomUserByVirtualId(Packet.PopInt());
            if (roomUserByVirtualId == null || roomUserByVirtualId.GetClient() == null || roomUserByVirtualId.GetClient().GetUser() == null)
            {
                return;
            }

            if (!roomUserByVirtualId.GetClient().GetUser().AcceptTrading && Session.GetUser().Rank < 3)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("user.tradedisabled", Session.Langue));
            }
            else if (roomUserByVirtualId.IsTransf || roomUserByUserId.IsTransf || roomUserByUserId.IsSpectator || roomUserByVirtualId.IsSpectator)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.trade.error.3", Session.Langue));
            }
            else
            {
                room.TryStartTrade(roomUserByUserId, roomUserByVirtualId);
            }
        }
    }
}