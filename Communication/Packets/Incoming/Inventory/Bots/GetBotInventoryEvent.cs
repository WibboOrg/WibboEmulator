using WibboEmulator.Communication.Packets.Outgoing.Inventory.Bots;
using WibboEmulator.Games.Clients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class GetBotInventoryEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session.GetUser() == null)
            {
                return;
            }

            Session.SendPacket(new BotInventoryComposer(Session.GetUser().GetInventoryComponent().GetBots()));
        }
    }
}
