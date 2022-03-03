using Butterfly.Game.Clients;
using Butterfly.Game.Roleplay.Player;
using Butterfly.Game.Rooms;
using Butterfly.Game.WebClients;

namespace Butterfly.Communication.Packets.Incoming.WebSocket
{
    internal class RpTrocAccepteEvent : IPacketWebEvent
    {
        public double Delay => 0;

        public void Parse(WebClient Session, ClientPacket Packet)
        {
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

            ButterflyEnvironment.GetGame().GetRoleplayManager().GetTrocManager().Accepte(Rp.TradeId, User.UserId);
        }
    }
}
