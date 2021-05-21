namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class MarketplaceConfigurationComposer : ServerPacket
    {
        public MarketplaceConfigurationComposer()
            : base(ServerPacketHeader.MARKETPLACE_CONFIG)
        {
            this.WriteBoolean(true);
            this.WriteInteger(0);//Min price.
            this.WriteInteger(0);//1?
            this.WriteInteger(0);//5?
            this.WriteInteger(1);// Prix Minimum
            this.WriteInteger(9999);//Max price.
            this.WriteInteger(48);
            this.WriteInteger(7);//Days.
        }
    }
}
