namespace WibboEmulator.Games.Chats.Commands.User.Room;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Polls;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class StartQuestion : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        var question = CommandManager.MergeParams(parameters, 1);

        if (string.IsNullOrWhiteSpace(question))
        {
            Session.SendWhisper("Votre question ne peut pas être vide");
            return;
        }

        room.SendPacket(new QuestionComposer(question));

        room.VotedNoCount = 0;
        room.VotedYesCount = 0;
    }
}
