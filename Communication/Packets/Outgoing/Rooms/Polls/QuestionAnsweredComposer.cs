namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Polls;

internal class QuestionAnsweredComposer : ServerPacket
{
    public QuestionAnsweredComposer(int userId, string value, int votedCountNo, int votedCountYes)
  : base(ServerPacketHeader.QUESTION_ANSWERED)
    {
        this.WriteInteger(userId);
        this.WriteString(value);
        this.WriteInteger(2);
        this.WriteString("0");
        this.WriteInteger(votedCountNo);
        this.WriteString("1");
        this.WriteInteger(votedCountYes);
    }
}
