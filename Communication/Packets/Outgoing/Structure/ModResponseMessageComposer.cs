namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class ModResponseMessageComposer : ServerPacket
    {
        public ModResponseMessageComposer()
            : base(ServerPacketHeader.ModToolIssueResponseAlertComposer)
        {

        }
    }
}
