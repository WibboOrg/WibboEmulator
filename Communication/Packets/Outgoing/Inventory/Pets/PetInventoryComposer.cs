namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Pets;
using WibboEmulator.Games.Rooms.AI;

internal class PetInventoryComposer : ServerPacket
{
    public PetInventoryComposer(ICollection<Pet> pets)
        : base(ServerPacketHeader.USER_PETS)
    {
        this.WriteInteger(1);
        this.WriteInteger(1);
        this.WriteInteger(pets.Count);
        foreach (var pet in pets.ToList())
        {
            this.WriteInteger(pet.PetId);
            this.WriteString(pet.Name);
            this.WriteInteger(pet.Type);
            this.WriteInteger(int.Parse(pet.Race));
            this.WriteString(pet.Color);
            this.WriteInteger(0);
            this.WriteInteger(0);
            this.WriteInteger(0);
        }
    }
}
