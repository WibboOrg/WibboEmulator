namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class ModeratorRoomChatlogMessageComposer : ServerPacket
    {
        public ModeratorRoomChatlogMessageComposer()
            : base(ServerPacketHeader.MODTOOL_ROOM_CHATLOG)
        {

        }
    }
}
