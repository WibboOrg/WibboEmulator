namespace Butterfly.Communication.Packets.Outgoing.Groups
{
    internal class RefreshFavouriteGroupComposer : ServerPacket
    {
        public RefreshFavouriteGroupComposer(int Id)
            : base(ServerPacketHeader.RefreshFavouriteGroupMessageComposer)
        {
            this.WriteInteger(Id);
        }
    }
}
