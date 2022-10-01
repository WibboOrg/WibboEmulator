using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Games.Clients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class GetCreditsInfoEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Session.SendPacket(new CreditBalanceComposer(Session.GetUser().Credits));
            Session.SendPacket(new ActivityPointsComposer(Session.GetUser().WibboPoints, Session.GetUser().LimitCoins));
        }
    }
}
