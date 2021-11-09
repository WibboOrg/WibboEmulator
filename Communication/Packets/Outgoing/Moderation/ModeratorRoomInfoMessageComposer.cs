namespace Butterfly.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorRoomInfoMessageComposer : ServerPacket
    {
        public ModeratorRoomInfoMessageComposer()
            : base(ServerPacketHeader.MODTOOL_ROOM_INFO)
        {

        }
    }
}
