using Butterfly.Game.Clients;
using Butterfly.Game.Roleplay.Player;
using Butterfly.Game.Rooms;
using Butterfly.Game.WebClients;

namespace Butterfly.Communication.Packets.Incoming.WebSocket
{
    internal class RpTrocRemoveItemEvent : IPacketWebEvent
    {
        public double Delay => 100;

        public void Parse(WebClient Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();

            Client Client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.UserId);
            if (Client == null || Client.GetUser() == null)
            {
                return;
            }

            Room Room = Client.GetUser().CurrentRoom;
            if (Room == null || !Room.IsRoleplay)
            {
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByUserId(Client.GetUser().Id);
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
