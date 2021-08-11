namespace Butterfly.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorInitMessageComposer : ServerPacket
    {
        public ModeratorInitMessageComposer()
            : base(ServerPacketHeader.MODERATION_TOOL)
        {

        }
    }
}
