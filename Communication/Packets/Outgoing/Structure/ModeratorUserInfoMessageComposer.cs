namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class ModeratorUserInfoMessageComposer : ServerPacket
    {
        public ModeratorUserInfoMessageComposer()
            : base(ServerPacketHeader.MODERATION_USER_INFO)
        {

        }
    }
}
