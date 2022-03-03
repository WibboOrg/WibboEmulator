using Butterfly.Game.Clients;
using Butterfly.Game.Roleplay.Player;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class InitTradeEvent : IPacketEvent
    {
        public double Delay => 5000;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room == null)
            {
                return;
            }

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

                ButterflyEnvironment.GetGame().GetRoleplayManager().GetTrocManager().AddTrade(room.RoomData.OwnerId, RoomUser.UserId, RoomUserTarget.UserId, RoomUser.GetUsername(), RoomUserTarget.GetUsername());
                return;
            }

            if (room.RoomData.TrocStatus == 0)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.trade.error.1", Session.Langue));
                return;
            }
            else if (room.RoomData.TrocStatus == 1 && !room.CheckRights(Session))
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.trade.error.2", Session.Langue));
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
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("user.tradedisabled", Session.Langue));
            }
            else if (roomUserByVirtualId.transformation || roomUserByUserId.transformation || roomUserByUserId.IsSpectator || roomUserByVirtualId.IsSpectator)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.trade.error.3", Session.Langue));
            }
            else
            {
                room.TryStartTrade(roomUserByUserId, roomUserByVirtualId);
            }
        }
    }
}