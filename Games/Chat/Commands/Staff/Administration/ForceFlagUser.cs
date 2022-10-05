namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Communication.Packets.Outgoing.Handshake;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class ForceFlagUser : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (Params.Length != 2)
        {
            return;
        }

        var clientByUsername = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(Params[1]);
        if (clientByUsername == null || clientByUsername.GetUser() == null)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", session.Langue));
            return;
        }

        clientByUsername.GetUser().CanChangeName = true;
        clientByUsername.SendPacket(new UserObjectComposer(clientByUsername.GetUser()));
        clientByUsername.SendNotification("Merci de procéder au changement de votre pseudonyme. Votre pseudonyme étant jugé comme innaproprié, vous serez banni sans aucun doute. \r\r Fermer cette fênetre et cliquez sur vous-même pour commencer à choisir un nouveau pseudonyme");
    }
}
