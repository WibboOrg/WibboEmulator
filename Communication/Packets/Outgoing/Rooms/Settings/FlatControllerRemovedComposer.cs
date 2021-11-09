using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class FlatControllerRemovedMessageComposer : ServerPacket
    {
        public FlatControllerRemovedMessageComposer(Room Instance, int UserId)
            : base(ServerPacketHeader.ROOM_RIGHTS_LIST_REMOVE)
        {
            WriteInteger(Instance.Id);
            WriteInteger(UserId);

        }
    }
}
