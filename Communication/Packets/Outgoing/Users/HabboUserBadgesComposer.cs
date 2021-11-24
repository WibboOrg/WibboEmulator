namespace Butterfly.Communication.Packets.Outgoing.Users
{
    internal class HabboUserBadgesComposer : ServerPacket
    {
        public HabboUserBadgesComposer()
            : base(ServerPacketHeader.USER_BADGES_CURRENT)
        {

        }
    }
}
