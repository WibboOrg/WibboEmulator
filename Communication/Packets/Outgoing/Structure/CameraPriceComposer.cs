namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class CameraPriceComposer : ServerPacket
    {
        public CameraPriceComposer(int Credits, int Duckets, int PublishDuckets)
            : base(ServerPacketHeader.CAMERA_PRICE)
        {
            this.WriteInteger(Credits);
            this.WriteInteger(Duckets);
            this.WriteInteger(PublishDuckets);
        }
    }
}