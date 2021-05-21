namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class BadgesMessageComposer : ServerPacket
    {
        public BadgesMessageComposer()
            : base(ServerPacketHeader.USER_BADGES)
        {

        }
    }
}
