namespace Butterfly.Communication.Packets.Outgoing.Rooms.Polls
{
    internal class QuestionFinishedComposer : ServerPacket
    {
        public QuestionFinishedComposer(int voteCountNo, int voteCountYes)
            : base(ServerPacketHeader.QUESTION_FINISHED)
        {
            this.WriteInteger(1);//PollId
            this.WriteInteger(2);//Count
            this.WriteString("0");//Négatif
            this.WriteInteger(voteCountNo);//Nombre
            this.WriteString("1");//Positif
            this.WriteInteger(voteCountYes);//Nombre
        }
    }
}
