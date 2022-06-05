using Wibbo.Communication.Packets.Outgoing.Handshake;
using Wibbo.Game.Rooms;
using Wibbo.Game.Clients;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class ForceFlagUser : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            Client clientByUsername = WibboEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (clientByUsername == null || clientByUsername.GetUser() == null)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
                return;
            }

            clientByUsername.GetUser().CanChangeName = true;
            clientByUsername.SendPacket(new UserObjectComposer(clientByUsername.GetUser()));
            clientByUsername.SendNotification("Merci de procéder au changement de votre pseudonyme. Votre pseudonyme étant jugé comme innaproprié, vous serez banni sans aucun doute. \r\r Fermer cette fênetre et cliquez sur vous-même pour commencer à choisir un nouveau pseudonyme");
        }
    }
}
