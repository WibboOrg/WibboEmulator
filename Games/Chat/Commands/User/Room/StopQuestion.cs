namespace WibboEmulator.Games.Chat.Commands.User.Room;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Polls;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class StopQuestion : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        Room.SendPacket(new QuestionFinishedComposer(Room.VotedNoCount, Room.VotedYesCount));

        session.SendWhisper("Question terminée!");
    }
}
