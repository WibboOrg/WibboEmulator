namespace Butterfly.Communication.Packets.Outgoing.Navigator
{
    internal class UpdateFavouriteRoomComposer : ServerPacket
    {
        public UpdateFavouriteRoomComposer()
            : base(ServerPacketHeader.USER_FAVORITE_ROOM)
        {

        }
    }
}
