namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class AddFilter : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (Params.Length != 2)
        {
            return;
        }

        WibboEnvironment.GetGame().GetChatManager().GetFilter().AddFilterPub(Params[1].ToLower());
        session.SendWhisper("Le mot" + Params[1] + " vient d'être ajouté au filtre");
    }
}