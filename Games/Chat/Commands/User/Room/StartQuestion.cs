namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Polls;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class StartQuestion : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        var Question = CommandManager.MergeParams(Params, 1);

        if (string.IsNullOrWhiteSpace(Question))
        {
            session.SendWhisper("Votre question ne peut pas être vide");
            return;
        }

        Room.SendPacket(new QuestionComposer(Question));

        Room.VotedNoCount = 0;
        Room.VotedYesCount = 0;
    }
}
