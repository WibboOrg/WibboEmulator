namespace Butterfly.Communication.Packets.Outgoing.Camera
{
    internal class CameraPurchaseSuccesfullComposer : ServerPacket
    {
        public CameraPurchaseSuccesfullComposer()
            : base(ServerPacketHeader.CameraPurchaseSuccesfullComposer)
        {
        }
    }
}