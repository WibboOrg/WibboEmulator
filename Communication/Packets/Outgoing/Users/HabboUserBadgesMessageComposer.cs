namespace Butterfly.Communication.Packets.Outgoing.Users
{
    internal class HabboUserBadgesMessageComposer : ServerPacket
    {
        public HabboUserBadgesMessageComposer()
            : base(ServerPacketHeader.USER_BADGES_CURRENT)
        {

        }
    }
}
