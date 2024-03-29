namespace WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Games.Catalogs.Pets;

internal sealed class SellablePetBreedsComposer : ServerPacket
{
    public SellablePetBreedsComposer(string petType, int petId, ICollection<PetRace> races)
         : base(ServerPacketHeader.CATALOG_RECEIVE_PET_BREEDS)
    {
        this.WriteString(petType);

        this.WriteInteger(races.Count);
        foreach (var race in races.ToList())
        {
            this.WriteInteger(petId); //type
            this.WriteInteger(race.PrimaryColour); //breedId
            this.WriteInteger(race.SecondaryColour); //paletteId
            this.WriteBoolean(true); //sellable
            this.WriteBoolean(false); //rare
        }
    }
}
