namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Pets;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Pets;
using WibboEmulator.Games.GameClients;

internal class GetPetInventoryEvent : IPacketEvent
{
    public double Delay => 5000;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.GetUser() == null)
        {
            return;
        }

        if (session.GetUser().InventoryComponent == null)
        {
            return;
        }

        session.GetUser().
        InventoryComponent.LoadInventory();

        session.SendPacket(new PetInventoryComposer(session.GetUser().InventoryComponent.GetPets()));
    }
}