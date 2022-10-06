namespace WibboEmulator.Communication.Packets.Outgoing.Groups;

internal class RefreshFavouriteGroupComposer : ServerPacket
{
    public RefreshFavouriteGroupComposer(int id)
        : base(ServerPacketHeader.REFRESH_FAVOURITE_GROUP_MESSAGE_COMPOSER) => this.WriteInteger(id);
}
