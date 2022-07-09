using Wibbo.Game.Clients;
using Wibbo.Game.Items;
using Wibbo.Game.Rooms;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class DiceOffEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
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
