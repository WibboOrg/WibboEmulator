using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Roleplay.Player;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class InitTradeEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            if (room.IsRoleplay)
            {
                RoomUser RoomUser = room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
                RoomUser RoomUserTarget = room.GetRoomUserManager().GetRoomUserByVirtualId(Packet.PopInt());
                if (RoomUser == null || RoomUser.GetClient() == null || RoomUser.GetClient().GetHabbo() == null)
                {
                    return;
                }

                if (RoomUserTarget == null || RoomUserTarget.GetClient() == null || RoomUserTarget.GetClient().GetHabbo() == null)
                {
                    return;
                }

                RolePlayer Rp = RoomUser.Roleplayer;
                if (Rp == null || Rp.TradeId > 0 || Rp.Dead || Rp.SendPrison || (Rp.PvpEnable && room.Pvp) || Rp.AggroTimer > 0)
                {
                    RoomUser.SendWhisperChat("Vous devez �tre en zone safe pour pouvoir troquer");
                    return;
                }

                RolePlayer RpTarget = RoomUserTarget.Roleplayer;
                if (RpTarget == null || RpTarget.TradeId > 0 || RpTarget.Dead || RpTarget.SendPrison || (RpTarget.PvpEnable && room.Pvp) || RpTarget.AggroTimer > 0)
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

            RoomUser roomUserByHabbo = room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            RoomUser roomUserByVirtualId = room.GetRoomUserManager().GetRoomUserByVirtualId(Packet.PopInt());
            if (roomUserByVirtualId == null || roomUserByVirtualId.GetClient() == null || roomUserByVirtualId.GetClient().GetHabbo() == null)
            {
                return;
            }

            if (!roomUserByVirtualId.GetClient().GetHabbo().AcceptTrading && Session.GetHabbo().Rank < 3)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("user.tradedisabled", Session.Langue));
            }
            else if (roomUserByVirtualId.transformation || roomUserByHabbo.transformation || roomUserByHabbo.IsSpectator || roomUserByVirtualId.IsSpectator)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.trade.error.3", Session.Langue));
            }
            else
            {
                room.TryStartTrade(roomUserByHabbo, roomUserByVirtualId);
            }
        }
    }
}