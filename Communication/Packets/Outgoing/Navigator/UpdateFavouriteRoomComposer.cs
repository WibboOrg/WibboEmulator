namespace Butterfly.Communication.Packets.Outgoing.Navigator
{
    internal class UpdateFavouriteRoomComposer : ServerPacket
    {
        public UpdateFavouriteRoomComposer(int roomId, bool added)
            : base(ServerPacketHeader.USER_FAVORITE_ROOM)
        {
            this.WriteInteger(roomId);
            this.WriteBoolean(added);
        }
    }
}
