namespace WibboEmulator.Communication.Packets.Outgoing.Groups;

internal sealed class RefreshFavouriteGroupComposer : ServerPacket
{
    public RefreshFavouriteGroupComposer(int id)
        : base(ServerPacketHeader.REFRESH_FAVOURITE_GROUP_MESSAGE_COMPOSER) => this.WriteInteger(id);
}
