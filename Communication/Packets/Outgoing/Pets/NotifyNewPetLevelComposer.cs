namespace WibboEmulator.Communication.Packets.Outgoing.Pets
{
    internal class NotifyNewPetLevelComposer : ServerPacket
    {
        public NotifyNewPetLevelComposer(int PetId, string Name, int Level)
            : base(ServerPacketHeader.PET_LEVEL_NOTIFICATION)
        {
            this.WriteInteger(PetId);
            this.WriteString(Name);
            this.WriteInteger(Level);
        }
    }
}
