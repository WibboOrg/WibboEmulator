namespace WibboEmulator.Games.Chats.Commands.Staff.Administration;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class GiveCoins : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        var clientByUsername = GameClientManager.GetClientByUsername(parameters[1]);
        if (clientByUsername != null)
        {
            if (int.TryParse(parameters[2], out var result))
            {
                clientByUsername.User.Credits = clientByUsername.User.Credits + result;
                clientByUsername.SendPacket(new CreditBalanceComposer(clientByUsername.User.Credits));
                clientByUsername.SendNotification(Session.User.Username + LanguageManager.TryGetValue("coins.awardmessage1", Session.Language) + result.ToString() + LanguageManager.TryGetValue("coins.awardmessage2", Session.Language));
                userRoom.SendWhisperChat(LanguageManager.TryGetValue("coins.updateok", Session.Language));
            }
            else
            {
                userRoom.SendWhisperChat(LanguageManager.TryGetValue("input.intonly", Session.Language));
            }
        }
        else
        {
            userRoom.SendWhisperChat(LanguageManager.TryGetValue("input.usernotfound", Session.Language));
        }
    }
}
