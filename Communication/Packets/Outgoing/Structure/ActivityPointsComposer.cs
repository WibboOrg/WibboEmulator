namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class ActivityPointsComposer : ServerPacket
    {
        public ActivityPointsComposer(int PixelsBalance, int SeasionalCurrency)
            : base(ServerPacketHeader.USER_CURRENCY)
        {
            this.WriteInteger(1);//Count
            {
                //WriteInteger(0);//Pixels
                //WriteInteger(PixelsBalance);

                this.WriteInteger(105);//Diamonds
                this.WriteInteger(SeasionalCurrency);
            }
        }
    }
}
