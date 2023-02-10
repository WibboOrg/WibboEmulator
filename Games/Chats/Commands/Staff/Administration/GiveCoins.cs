namespace WibboEmulator.Games.Chats.Commands.Staff.Administration;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class GiveCoins : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var clientByUsername = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(parameters[1]);
        if (clientByUsername != null)
        {
            if (int.TryParse(parameters[2], out var result))
            {
                clientByUsername.User.Credits = clientByUsername.User.Credits + result;
                clientByUsername.SendPacket(new CreditBalanceComposer(clientByUsername.User.Credits));
                clientByUsername.SendNotification(session.User.Username + WibboEnvironment.GetLanguageManager().TryGetValue("coins.awardmessage1", session.Langue) + result.ToString() + WibboEnvironment.GetLanguageManager().TryGetValue("coins.awardmessage2", session.Langue));
                session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("coins.updateok", session.Langue));
            }
            else
            {
                session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.intonly", session.Langue));
            }
        }
        else
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", session.Langue));
        }
    }
}
