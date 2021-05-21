using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;
using Butterfly.HabboHotel.Rooms.Wired;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class SaveWiredConfigEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
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

            WiredSaver.HandleSave(Session, Packet.PopInt(), Session.GetHabbo().CurrentRoom, Packet);
        }
    }
}
