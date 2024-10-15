namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Bots;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Bots;
using WibboEmulator.Games.GameClients;

internal sealed class GetBotInventoryEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session.User == null)
        {
            return;
        }

        Session.SendPacket(new BotInventoryComposer(Session.User.InventoryComponent.Bots));
    }
}
