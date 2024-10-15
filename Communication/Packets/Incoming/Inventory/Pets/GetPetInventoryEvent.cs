namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Pets;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Pets;
using WibboEmulator.Games.GameClients;

internal sealed class GetPetInventoryEvent : IPacketEvent
{
    public double Delay => 5000;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session.User == null)
        {
            return;
        }

        if (Session.User.InventoryComponent == null)
        {
            return;
        }

        Session.User.InventoryComponent.LoadInventory();

        Session.SendPacket(new PetInventoryComposer(Session.User.InventoryComponent.Pets));
    }
}