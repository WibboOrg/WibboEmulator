namespace WibboEmulator.Communication.Packets.Incoming.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Games.GameClients;

internal class GetSellablePetBreedsEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var type = packet.PopString();

        var item = WibboEnvironment.GetGame().GetItemManager().GetItemByName(type);
        if (item == null)
        {
            return;
        }

        var petId = item.SpriteId;

        session.SendPacket(new SellablePetBreedsComposer(type, petId, WibboEnvironment.GetGame().GetCatalog().GetRacesForRaceId(petId)));
    }
}
