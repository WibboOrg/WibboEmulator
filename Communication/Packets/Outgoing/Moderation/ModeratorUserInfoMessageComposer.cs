namespace Butterfly.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorUserInfoMessageComposer : ServerPacket
    {
        public ModeratorUserInfoMessageComposer()
            : base(ServerPacketHeader.MODERATION_USER_INFO)
        {
        }
    }
}
