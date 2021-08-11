namespace Butterfly.Communication.Packets.Outgoing.Navigator
{
    internal class UpdateFavouriteRoomMessageComposer : ServerPacket
    {
        public UpdateFavouriteRoomMessageComposer()
            : base(ServerPacketHeader.USER_FAVORITE_ROOM)
        {

        }
    }
}
