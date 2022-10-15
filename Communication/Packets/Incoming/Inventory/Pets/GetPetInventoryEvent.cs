namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Pets;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Pets;
using WibboEmulator.Games.GameClients;

internal class GetPetInventoryEvent : IPacketEvent
{
    public double Delay => 5000;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.User == null)
        {
            return;
        }

        if (session.User.InventoryComponent == null)
        {
            return;
        }

        session.
        User.
        InventoryComponent.LoadInventory();

        session.SendPacket(new PetInventoryComposer(session.User.InventoryComponent.GetPets()));
    }
}