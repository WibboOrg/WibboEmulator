namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Polls;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Polls;
using WibboEmulator.Games.GameClients;

internal class AnswerPollEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        var User = room.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
        if (User == null)
        {
            return;
        }

        var Id = packet.PopInt();
        var QuestionId = packet.PopInt();

        var Count = packet.PopInt();//Count

        var Value = "0";
        for (var i = 0; i < Count; i++)
        {
            Value = packet.PopString();
        }

        Value = Value is not "0" and not "1" ? "0" : Value;

        if (Value == "0")
        {
            room.VotedNoCount++;
        }
        else
        {
            room.VotedYesCount++;
        }

        room.SendPacket(new QuestionAnsweredComposer(session.GetUser().Id, Value, room.VotedNoCount, room.VotedYesCount));

        var WiredCode = Value == "0" ? "QUESTION_NO" : "QUESTION_YES";
        if (room.AllowsShous(User, WiredCode))
        {
            User.SendWhisperChat(WiredCode, false);
        }
    }
}