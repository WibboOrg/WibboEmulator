using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class DiceOffEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            Item roomItem = room.GetRoomItemHandler().GetItem(Packet.PopInt());
            if (roomItem == null)
            {
                return;
            }

            bool UserHasRights = false;
            if (room.CheckRights(Session))
            {
                UserHasRights = true;
            }

            roomItem.Interactor.OnTrigger(Session, roomItem, -1, UserHasRights, false);
            roomItem.OnTrigger(room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id));
        }
    }
}
