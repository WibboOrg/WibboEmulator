namespace Butterfly.Communication.Packets.Outgoing.Structure
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
