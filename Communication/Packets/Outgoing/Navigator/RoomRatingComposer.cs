namespace WibboEmulator.Communication.Packets.Outgoing.Navigator;

internal sealed class RoomRatingComposer : ServerPacket
{
    public RoomRatingComposer(int score, bool canVote)
        : base(ServerPacketHeader.ROOM_SCORE)
    {
        this.WriteInteger(score);
        this.WriteBoolean(canVote);
    }
}
