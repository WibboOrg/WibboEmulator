using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class DiceOffEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room room))
                return;

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
