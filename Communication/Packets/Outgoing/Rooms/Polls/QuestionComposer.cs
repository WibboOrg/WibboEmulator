namespace Butterfly.Communication.Packets.Outgoing.Rooms.Polls
{
    internal class QuestionComposer : ServerPacket
    {
        public QuestionComposer(string answer)
            : base(ServerPacketHeader.QUESTION)
        {
            this.WriteString("MATCHING_POLL"); //Type
            this.WriteInteger(1);//pollId
            this.WriteInteger(1);//questionId
            this.WriteInteger(60);//Duration
            this.WriteInteger(1); //id
            this.WriteInteger(1);//number
            this.WriteInteger(3);//type (1 ou 2)
            this.WriteString(answer);//content
            this.WriteInteger(0);
            this.WriteInteger(0);
            //MessageTwo.WriteString("0");
            //MessageTwo.WriteString("1");
        }
    }
}
