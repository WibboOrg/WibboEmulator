using Butterfly.Game.Clients;
using Butterfly.Game.Roleplay.Player;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.RolePlay.Troc
{
    internal class RpTrocAddItemEvent : IPacketEvent
    {
        public double Delay => 100;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();

            if (Session == null || Session.GetUser() == null)
            {
                return;
            }

            Room Room = Session.GetUser().CurrentRoom;
            if (Room == null || !Room.IsRoleplay)
            {
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
            if (User == null)
            {
                return;
            }

            RolePlayer Rp = User.Roleplayer;
            if (Rp == null || Rp.TradeId == 0)
            {
                return;
            }

            ButterflyEnvironment.GetGame().GetRoleplayManager().GetTrocManager().AddItem(Rp.TradeId, User.UserId, ItemId);
        }
    }
}
