namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class HabboUserBadgesMessageComposer : ServerPacket
    {
        public HabboUserBadgesMessageComposer()
            : base(ServerPacketHeader.USER_BADGES_CURRENT)
        {

        }
    }
}
