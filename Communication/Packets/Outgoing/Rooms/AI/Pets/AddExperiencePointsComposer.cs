namespace Butterfly.Communication.Packets.Outgoing.Rooms.AI.Pets
{
    internal class AddExperiencePointsComposer : ServerPacket
    {
        public AddExperiencePointsComposer()
            : base(ServerPacketHeader.AddExperiencePointsMessageComposer)
        {

        }
    }
}
