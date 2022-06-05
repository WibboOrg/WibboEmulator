using Wibbo.Communication.Packets.Outgoing.Rooms.Polls;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class AnswerPollEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            RoomUser User = room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
            if (User == null)
            {
                return;
            }

            int Id = Packet.PopInt();
            int QuestionId = Packet.PopInt();

            int Count = Packet.PopInt();//Count

            string Value = "0";
            for (int i = 0; i < Count; i++)
            {
                Value = Packet.PopString();
            }

            Value = (Value != "0" && Value != "1") ? "0" : Value;

            if (Value == "0")
            {
                room.VotedNoCount++;
            }
            else
            {
                room.VotedYesCount++;
            }

            room.SendPacket(new QuestionAnsweredComposer(Session.GetUser().Id, Value, room.VotedNoCount, room.VotedYesCount));

            string WiredCode = (Value == "0") ? "QUESTION_NO" : "QUESTION_YES";
            if (room.AllowsShous(User, WiredCode))
            {
                User.SendWhisperChat(WiredCode, false);
            }
        }
    }
}