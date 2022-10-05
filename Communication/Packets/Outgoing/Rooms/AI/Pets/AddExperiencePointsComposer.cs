namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.AI.Pets;

internal class AddExperiencePointsComposer : ServerPacket
{
    public AddExperiencePointsComposer(int PetId, int VirtualId, int Amount)
        : base(ServerPacketHeader.PET_EXPERIENCE)
    {
        this.WriteInteger(PetId);
        this.WriteInteger(VirtualId);
        this.WriteInteger(Amount);
    }
}
