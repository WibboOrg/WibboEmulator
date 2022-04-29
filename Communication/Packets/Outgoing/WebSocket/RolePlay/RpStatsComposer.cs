namespace Butterfly.Communication.Packets.Outgoing.WebSocket
{
    internal class RpStatsComposer : ServerPacket
    {
        public RpStatsComposer(int pRpId, int pHealth, int pHealMax, int pEnergy, int pHygiene, int pMoney, int pMoney1, int pMoney2, int pMoney3, int pMoney4, int pMunition, int pLevel)
            : base(ServerPacketHeader.RP_STATS)
        {
            this.WriteInteger(pRpId);
            this.WriteInteger(pHealth);
            this.WriteInteger(pHealMax);
            this.WriteInteger(pEnergy);
            this.WriteInteger(pHygiene);
            this.WriteInteger(pMoney);
            this.WriteInteger(pMoney1);
            this.WriteInteger(pMoney2);
            this.WriteInteger(pMoney3);
            this.WriteInteger(pMoney4);
            this.WriteInteger(pMunition);
            this.WriteInteger(pLevel);
        }
    }
}
