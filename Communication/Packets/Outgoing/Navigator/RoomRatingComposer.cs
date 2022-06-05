namespace Wibbo.Communication.Packets.Outgoing.Navigator
{
    internal class RoomRatingComposer : ServerPacket
    {
        public RoomRatingComposer(int Score, bool CanVote)
            : base(ServerPacketHeader.ROOM_SCORE)
        {
            this.WriteInteger(Score);
            this.WriteBoolean(CanVote);
        }
    }
}
