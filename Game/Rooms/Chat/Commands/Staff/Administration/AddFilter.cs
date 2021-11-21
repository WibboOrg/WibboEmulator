using Butterfly.Game.GameClients;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class AddFilter : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            ButterflyEnvironment.GetGame().GetChatManager().GetFilter().AddFilterPub(Params[1].ToLower());
            UserRoom.SendWhisperChat("Le mot" + Params.ToString[1] + " vient d'être ajouté au filtre");
        }
    }
}