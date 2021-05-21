namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class BuildersClubMembershipComposer : ServerPacket
    {
        public BuildersClubMembershipComposer()
            : base(ServerPacketHeader.BUILDERS_CLUB_EXPIRED)
        {
            this.WriteInteger(99999999);
            this.WriteInteger(100);
            this.WriteInteger(2);
            this.WriteInteger(99999999);
        }
    }
}
