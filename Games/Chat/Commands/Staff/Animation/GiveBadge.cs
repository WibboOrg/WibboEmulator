using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class GiveBadge : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            GameClient clientByUsername = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(Params[1]);
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
