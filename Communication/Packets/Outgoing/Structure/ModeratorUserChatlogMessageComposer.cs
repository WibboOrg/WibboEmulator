namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class ModeratorUserChatlogMessageComposer : ServerPacket
    {
        public ModeratorUserChatlogMessageComposer()
            : base(ServerPacketHeader.MODTOOL_USER_CHATLOG)
        {

        }
    }
}
