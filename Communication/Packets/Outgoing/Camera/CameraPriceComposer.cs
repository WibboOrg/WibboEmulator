namespace Butterfly.Communication.Packets.Outgoing.Camera
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