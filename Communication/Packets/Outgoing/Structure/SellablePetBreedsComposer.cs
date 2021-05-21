using Butterfly.HabboHotel.Catalog.Pets;
using System.Collections.Generic;
using System.Linq;

namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class SellablePetBreedsComposer : ServerPacket
    {
        public SellablePetBreedsComposer(string PetType, int PetId, ICollection<PetRace> Races)
             : base(ServerPacketHeader.SellablePetBreedsMessageComposer)
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
