namespace Butterfly.Communication.Packets.Outgoing.Pets
{
    internal class NotifyNewPetLevelComposer : ServerPacket
    {
        public NotifyNewPetLevelComposer()
            : base(ServerPacketHeader.PET_LEVEL_NOTIFICATION)
        {

        }
    }
}
