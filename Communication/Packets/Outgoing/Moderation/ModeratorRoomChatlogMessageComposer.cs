namespace Butterfly.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorRoomChatlogMessageComposer : ServerPacket
    {
        public ModeratorRoomChatlogMessageComposer()
            : base(ServerPacketHeader.MODTOOL_ROOM_CHATLOG)
        {

        }
    }
}
