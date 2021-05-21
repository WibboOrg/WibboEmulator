namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class CameraPurchaseSuccesfullComposer : ServerPacket
    {
        public CameraPurchaseSuccesfullComposer()
            : base(ServerPacketHeader.CameraPurchaseSuccesfullComposer)
        {
        }
    }
}