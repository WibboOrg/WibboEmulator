using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class GiveCoins : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room currentRoom = Session.GetHabbo().CurrentRoom;
            Client clientByUsername = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (clientByUsername != null)
            {
                if (int.TryParse(Params[2], out int result))
                {
                    clientByUsername.GetHabbo().Credits = clientByUsername.GetHabbo().Credits + result;
                    clientByUsername.GetHabbo().UpdateCreditsBalance();
                    clientByUsername.SendNotification(Session.GetHabbo().Username + ButterflyEnvironment.GetLanguageManager().TryGetValue("coins.awardmessage1", Session.Langue) + result.ToString() + ButterflyEnvironment.GetLanguageManager().TryGetValue("coins.awardmessage2", Session.Langue));
                    Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("coins.updateok", Session.Langue));
                }
                else
                {
                    Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.intonly", Session.Langue));
                }
            }
            else
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            }
        }
    }
}