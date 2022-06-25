namespace WibboEmulator.Communication.Packets.Outgoing.Camera
{
    internal class CameraPriceComposer : ServerPacket
    {
        public CameraPriceComposer(int Credits, int Duckets, int PublishDuckets)
            : base(ServerPacketHeader.INIT_CAMERA)
        {
            this.WriteInteger(Credits);
            this.WriteInteger(Duckets);
            this.WriteInteger(PublishDuckets);
        }
    }
}