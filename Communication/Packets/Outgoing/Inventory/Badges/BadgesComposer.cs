namespace Butterfly.Communication.Packets.Outgoing.Inventory.Badges
{
    internal class BadgesComposer : ServerPacket
    {
        public BadgesComposer()
            : base(ServerPacketHeader.USER_BADGES)
        {

        }
    }
}
