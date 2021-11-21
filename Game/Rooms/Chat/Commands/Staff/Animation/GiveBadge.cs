using Butterfly.Communication.Packets.Outgoing.Users;
using Butterfly.Game.Clients;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class GiveBadge : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Client clientByUsername = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (clientByUsername != null)
            {
                /*if (Session.Langue != clientByUsername.Langue)
                {
                    UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue(string.Format("cmd.authorized.langue.user", clientByUsername.Langue), Session.Langue));
                    return;
                }*/

                string BadgeCode = Params[2];
                clientByUsername.GetHabbo().GetBadgeComponent().GiveBadge(BadgeCode, true);
                clientByUsername.SendPacket(new ReceiveBadgeComposer(BadgeCode));
            }
            else
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            }
        }
    }
}
