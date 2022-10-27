namespace WibboEmulator.Games.Chats.Commands.User.Room;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Polls;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class StartQuestion : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var question = CommandManager.MergeParams(parameters, 1);

        if (string.IsNullOrWhiteSpace(question))
        {
            session.SendWhisper("Votre question ne peut pas être vide");
            return;
        }

        room.SendPacket(new QuestionComposer(question));

        room.VotedNoCount = 0;
        room.VotedYesCount = 0;
    }
}
