namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class UpdateFavouriteRoomMessageComposer : ServerPacket
    {
        public UpdateFavouriteRoomMessageComposer()
            : base(ServerPacketHeader.USER_FAVORITE_ROOM)
        {

        }
    }
}
