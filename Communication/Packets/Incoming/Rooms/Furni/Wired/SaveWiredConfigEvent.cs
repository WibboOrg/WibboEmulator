using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;
using Butterfly.Game.Rooms.Wired;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class SaveWiredConfigEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            Room room = Session.GetHabbo().CurrentRoom;
            if (room == null)
            {
                return;
            }

            if (!room.CheckRights(Session) && !room.CheckRights(Session, true))
            {
                return;
            }

            int itemId = Packet.PopInt();

            WiredSaver.HandleSave(Session, itemId, Session.GetHabbo().CurrentRoom, Packet);
        }
    }
}
