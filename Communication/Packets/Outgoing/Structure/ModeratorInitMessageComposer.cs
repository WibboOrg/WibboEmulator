namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class ModeratorInitMessageComposer : ServerPacket
    {
        public ModeratorInitMessageComposer()
            : base(ServerPacketHeader.MODERATION_TOOL)
        {

        }
    }
}
