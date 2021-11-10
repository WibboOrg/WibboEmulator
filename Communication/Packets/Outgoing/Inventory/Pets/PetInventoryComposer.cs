using Butterfly.Game.Pets;
using System.Collections.Generic;
using System.Linq;

namespace Butterfly.Communication.Packets.Outgoing.Inventory.Pets
{
    internal class PetInventoryComposer : ServerPacket
    {
        public PetInventoryComposer(ICollection<Pet> Pets)
            : base(ServerPacketHeader.USER_PETS)
        {
            this.WriteInteger(1);
            this.WriteInteger(1);
            this.WriteInteger(Pets.Count);
            foreach (Pet Pet in Pets.ToList())
            {
                this.WriteInteger(Pet.PetId);
                this.WriteString(Pet.Name);
                this.WriteInteger(Pet.Type);
                this.WriteInteger(int.Parse(Pet.Race));
                this.WriteString(Pet.Color);
                this.WriteInteger(0);
                this.WriteInteger(0);
                this.WriteInteger(0);
            }
        }
    }
}
