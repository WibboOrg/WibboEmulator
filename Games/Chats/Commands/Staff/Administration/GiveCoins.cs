namespace WibboEmulator.Games.Chats.Commands.Staff.Administration;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class GiveCoins : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var clientByUsername = GameClientManager.GetClientByUsername(parameters[1]);
        if (clientByUsername != null)
        {
            if (int.TryParse(parameters[2], out var result))
            {
                clientByUsername.User.Credits = clientByUsername.User.Credits + result;
                clientByUsername.SendPacket(new CreditBalanceComposer(clientByUsername.User.Credits));
                clientByUsername.SendNotification(session.User.Username + LanguageManager.TryGetValue("coins.awardmessage1", session.Language) + result.ToString() + LanguageManager.TryGetValue("coins.awardmessage2", session.Language));
                userRoom.SendWhisperChat(LanguageManager.TryGetValue("coins.updateok", session.Language));
            }
            else
            {
                userRoom.SendWhisperChat(LanguageManager.TryGetValue("input.intonly", session.Language));
            }
        }
        else
        {
            userRoom.SendWhisperChat(LanguageManager.TryGetValue("input.usernotfound", session.Language));
        }
    }
}
