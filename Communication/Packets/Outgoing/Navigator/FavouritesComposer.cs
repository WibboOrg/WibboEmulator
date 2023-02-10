namespace WibboEmulator.Communication.Packets.Outgoing.Navigator;

internal sealed class FavouritesComposer : ServerPacket
{
    public FavouritesComposer(List<int> favouriteIDs)
        : base(ServerPacketHeader.USER_FAVORITE_ROOM_COUNT)
    {
        this.WriteInteger(30);
        this.WriteInteger(favouriteIDs.Count);

        foreach (var roomId in favouriteIDs)
        {
            this.WriteInteger(roomId);
        }
    }
}
