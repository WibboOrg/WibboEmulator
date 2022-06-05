namespace Butterfly.Communication.Packets.Outgoing.Navigator
{
    internal class FavouritesComposer : ServerPacket
    {
        public FavouritesComposer(List<int> favouriteIDs)
            : base(ServerPacketHeader.USER_FAVORITE_ROOM_COUNT)
        {
            this.WriteInteger(30);
            this.WriteInteger(favouriteIDs.Count);

            foreach (int RoomId in favouriteIDs)
            {
                this.WriteInteger(RoomId);
            }
        }
    }
}
