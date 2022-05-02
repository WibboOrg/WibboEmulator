namespace Butterfly.Communication.Packets.Outgoing.WebSocket
{
    internal class RpStatsComposer : ServerPacket
    {
        public RpStatsComposer(int pRpId, int pHealth, int pHealMax, int pEnergy, int pMoney, int pMunition, int pLevel)
            : base(ServerPacketHeader.RP_STATS)
        {
            this.WriteInteger(pRpId);
            this.WriteInteger(pHealth);
            this.WriteInteger(pHealMax);
            this.WriteInteger(pEnergy);
            this.WriteInteger(pMoney);
            this.WriteInteger(pMunition);
            this.WriteInteger(pLevel);
        }
    }
}
