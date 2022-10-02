using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class GiveCoins : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            GameClient clientByUsername = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(Params[1]);
            if (clientByUsername != null)
            {
                if (int.TryParse(Params[2], out int result))
                {
                    clientByUsername.GetUser().Credits = clientByUsername.GetUser().Credits + result;
                    clientByUsername.SendPacket(new CreditBalanceComposer(clientByUsername.GetUser().Credits));
                    clientByUsername.SendNotification(Session.GetUser().Username + WibboEnvironment.GetLanguageManager().TryGetValue("coins.awardmessage1", Session.Langue) + result.ToString() + WibboEnvironment.GetLanguageManager().TryGetValue("coins.awardmessage2", Session.Langue));
                    Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("coins.updateok", Session.Langue));
                }
                else
                {
                    Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.intonly", Session.Langue));
                }
            }
            else
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            }
        }
    }
}
