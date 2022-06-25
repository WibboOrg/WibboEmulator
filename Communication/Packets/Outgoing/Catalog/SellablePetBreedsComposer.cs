using WibboEmulator.Game.Catalog.Pets;

namespace WibboEmulator.Communication.Packets.Outgoing.Catalog
{
    internal class SellablePetBreedsComposer : ServerPacket
    {
        public SellablePetBreedsComposer(string PetType, int PetId, ICollection<PetRace> Races)
             : base(ServerPacketHeader.CATALOG_RECEIVE_PET_BREEDS)
        {
            this.WriteString(PetType);

            this.WriteInteger(Races.Count);
            foreach (PetRace Race in Races.ToList())
            {
                this.WriteInteger(PetId);
                this.WriteInteger(Race.PrimaryColour);
                this.WriteInteger(Race.SecondaryColour);
                this.WriteBoolean(Race.HasPrimaryColour);
                this.WriteBoolean(Race.HasSecondaryColour);
            }
        }
    }
}
