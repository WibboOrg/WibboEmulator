using Butterfly.Game.Clients;
using Butterfly.Game.Roleplay.Player;
using Butterfly.Game.Rooms;
using Butterfly.Game.WebClients;

namespace Butterfly.Communication.Packets.Incoming.WebSocket
{
    internal class RpTrocRemoveItemEvent : IPacketWebEvent
    {
        public double Delay => 0;

        public void Parse(WebClient Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();

            Client Client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.UserId);
            if (Client == null || Client.GetHabbo() == null)
            {
                return;
            }

            Room Room = Client.GetHabbo().CurrentRoom;
            if (Room == null || !Room.IsRoleplay)
            {
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabboId(Client.GetHabbo().Id);
            if (User == null)
            {
                return;
            }

            RolePlayer Rp = User.Roleplayer;
            if (Rp == null || Rp.TradeId == 0)
            {
                return;
            }

            ButterflyEnvironment.GetGame().GetRoleplayManager().GetTrocManager().RemoveItem(Rp.TradeId, User.UserId, ItemId);
        }
    }
}
