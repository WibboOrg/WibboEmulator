namespace Butterfly.Communication.Packets.Outgoing.Inventory.Badges
{
    internal class BadgesMessageComposer : ServerPacket
    {
        public BadgesMessageComposer()
            : base(ServerPacketHeader.USER_BADGES)
        {

        }
    }
}
