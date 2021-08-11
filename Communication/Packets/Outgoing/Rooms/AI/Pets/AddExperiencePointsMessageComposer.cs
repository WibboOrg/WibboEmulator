namespace Butterfly.Communication.Packets.Outgoing.Rooms.AI.Pets
{
    internal class AddExperiencePointsMessageComposer : ServerPacket
    {
        public AddExperiencePointsMessageComposer()
            : base(ServerPacketHeader.AddExperiencePointsMessageComposer)
        {

        }
    }
}
