namespace WibboEmulator.Communication.Packets.Incoming.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Games.Catalogs;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal sealed class GetSellablePetBreedsEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var type = packet.PopString();

        var item = ItemManager.GetItemByName(type);
        if (item == null)
        {
            return;
        }

        var petId = item.SpriteId;

        Session.SendPacket(new SellablePetBreedsComposer(type, petId, CatalogManager.GetRacesForRaceId(petId)));
    }
}
