namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class ModeratorRoomInfoMessageComposer : ServerPacket
    {
        public ModeratorRoomInfoMessageComposer()
            : base(ServerPacketHeader.MODTOOL_ROOM_INFO)
        {

        }
    }
}
