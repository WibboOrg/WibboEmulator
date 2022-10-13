namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Bots;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Bots;
using WibboEmulator.Games.GameClients;

internal class GetBotInventoryEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.GetUser() == null)
        {
            return;
        }

        session.SendPacket(new BotInventoryComposer(session.GetUser().InventoryComponent.GetBots()));
    }
}
