namespace WibboEmulator.Games.Chats.Commands.Staff.Administration;
using WibboEmulator.Communication.Packets.Outgoing.Handshake;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class ForceFlagUser : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var clientByUsername = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(parameters[1]);
        if (clientByUsername == null || clientByUsername.User == null)
        {
            userRoom.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", session.Langue));
            return;
        }

        clientByUsername.User.CanChangeName = true;
        clientByUsername.SendPacket(new UserObjectComposer(clientByUsername.User));
        clientByUsername.SendNotification("Merci de procéder au changement de votre pseudonyme. Votre pseudonyme étant jugé comme innaproprié, vous serez banni sans aucun doute. \r\r Fermer cette fênetre et cliquez sur vous-même pour commencer à choisir un nouveau pseudonyme");
    }
}
