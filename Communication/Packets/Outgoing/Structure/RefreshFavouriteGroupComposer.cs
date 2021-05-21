namespace Butterfly.Communication.Packets.Outgoing.Structure
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
