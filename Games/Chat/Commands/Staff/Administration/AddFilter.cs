using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class AddFilter : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            WibboEnvironment.GetGame().GetChatManager().GetFilter().AddFilterPub(Params[1].ToLower());
            Session.SendWhisper("Le mot" + Params[1] + " vient d'être ajouté au filtre");
        }
    }
}