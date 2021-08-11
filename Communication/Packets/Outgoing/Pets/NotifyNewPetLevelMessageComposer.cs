namespace Butterfly.Communication.Packets.Outgoing.Pets
{
    internal class NotifyNewPetLevelMessageComposer : ServerPacket
    {
        public NotifyNewPetLevelMessageComposer()
            : base(ServerPacketHeader.PetLevelUpComposer)
        {

        }
    }
}
