using Wibbo.Communication.Packets.Outgoing.Users;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class GiveBadge : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Client clientByUsername = WibboEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (clientByUsername != null)
            {
                /*if (Session.Langue != clientByUsername.Langue)
                {
                    Session.SendWhisper(ButterflyEnvironment.GetLanguageManager().TryGetValue(string.Format("cmd.authorized.langue.user", clientByUsername.Langue), Session.Langue));
                    return;
                }*/

                string BadgeCode = Params[2];
                clientByUsername.GetUser().GetBadgeComponent().GiveBadge(BadgeCode, true);
                clientByUsername.SendPacket(new ReceiveBadgeComposer(BadgeCode));
            }
            else
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            }
        }
    }
}
