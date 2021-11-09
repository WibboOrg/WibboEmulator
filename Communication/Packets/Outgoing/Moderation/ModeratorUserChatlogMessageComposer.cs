namespace Butterfly.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorUserChatlogMessageComposer : ServerPacket
    {
        public ModeratorUserChatlogMessageComposer()
            : base(ServerPacketHeader.MODTOOL_USER_CHATLOG)
        {

        }
    }
}
