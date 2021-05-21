namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class NotifyNewPetLevelMessageComposer : ServerPacket
    {
        public NotifyNewPetLevelMessageComposer()
            : base(ServerPacketHeader.PetLevelUpComposer)
        {

        }
    }
}
