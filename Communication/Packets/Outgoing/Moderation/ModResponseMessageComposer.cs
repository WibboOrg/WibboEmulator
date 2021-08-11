namespace Butterfly.Communication.Packets.Outgoing.Moderation
{
    internal class ModResponseMessageComposer : ServerPacket
    {
        public ModResponseMessageComposer()
            : base(ServerPacketHeader.ModToolIssueResponseAlertComposer)
        {

        }
    }
}
