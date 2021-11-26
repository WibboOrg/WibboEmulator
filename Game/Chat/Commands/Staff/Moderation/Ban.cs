using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class Ban : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 2)
            {
                return;
            }

            Client clientByUsername = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (clientByUsername == null || clientByUsername.GetHabbo() == null)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
                return;
            }

            if (clientByUsername.GetHabbo().Rank >= Session.GetHabbo().Rank)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("action.notallowed", Session.Langue));
                return;
            }

            int.TryParse(Params[2], out int num);
            if (num <= 600)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("ban.toolesstime", Session.Langue));
            }
            else
            {
                string Raison = CommandManager.MergeParams(Params, 3);
                ButterflyEnvironment.GetGame().GetClientManager().BanUser(clientByUsername, Session.GetHabbo().Username, num, Raison, false, false);
                UserRoom.SendWhisperChat("Tu as bannit " + clientByUsername.GetHabbo().Username + " pour" + Raison + "!");
                if (Session.Antipub(Raison, "<CMD>", Room.Id))
                {
                    return;
                }
            }
        }
    }
}
