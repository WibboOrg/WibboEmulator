namespace WibboEmulator.Communication.Packets.Outgoing.Pets;

internal class NotifyNewPetLevelComposer : ServerPacket
{
    public NotifyNewPetLevelComposer(int petId, string name, int level)
        : base(ServerPacketHeader.PET_LEVEL_NOTIFICATION)
    {
        this.WriteInteger(petId);
        this.WriteString(name);
        this.WriteInteger(level);
    }
}
