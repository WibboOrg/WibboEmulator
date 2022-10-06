namespace WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Games.Catalog.Pets;

internal class SellablePetBreedsComposer : ServerPacket
{
    public SellablePetBreedsComposer(string petType, int petId, ICollection<PetRace> races)
         : base(ServerPacketHeader.CATALOG_RECEIVE_PET_BREEDS)
    {
        this.WriteString(petType);

        this.WriteInteger(races.Count);
        foreach (var race in races.ToList())
        {
            this.WriteInteger(petId);
            this.WriteInteger(race.PrimaryColour);
            this.WriteInteger(race.SecondaryColour);
            this.WriteBoolean(race.HasPrimaryColour);
            this.WriteBoolean(race.HasSecondaryColour);
        }
    }
}
