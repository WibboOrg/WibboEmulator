namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class AddExperiencePointsMessageComposer : ServerPacket
    {
        public AddExperiencePointsMessageComposer()
            : base(ServerPacketHeader.AddExperiencePointsMessageComposer)
        {

        }
    }
}
