namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Furni;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Games.GameClients;

internal class RequestFurniInventoryEvent : IPacketEvent
{
    public double Delay => 5000;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.GetUser() == null)
        {
            return;
        }

        if (session.GetUser().GetInventoryComponent() == null)
        {
            return;
        }

        session.GetUser().GetInventoryComponent().LoadInventory();

        var items = session.GetUser().GetInventoryComponent().GetWallAndFloor;
        session.SendPacket(new FurniListComposer(items.ToList(), 1, 0));
    }
}
