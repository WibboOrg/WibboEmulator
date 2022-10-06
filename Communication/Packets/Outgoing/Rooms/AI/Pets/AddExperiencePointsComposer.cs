namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.AI.Pets;

internal class AddExperiencePointsComposer : ServerPacket
{
    public AddExperiencePointsComposer(int petId, int virtualId, int amount)
        : base(ServerPacketHeader.PET_EXPERIENCE)
    {
        this.WriteInteger(petId);
        this.WriteInteger(virtualId);
        this.WriteInteger(amount);
    }
}
