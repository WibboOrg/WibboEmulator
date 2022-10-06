namespace WibboEmulator.Games.Chat.Commands.Staff.Animation;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class GiveBadge : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        var clientByUsername = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(parameters[1]);
        if (clientByUsername != null)
        {
            /*if (session.Langue != clientByUsername.Langue)
            {
                session.SendWhisper(ButterflyEnvironment.GetLanguageManager().TryGetValue(string.Format("cmd.authorized.langue.user", clientByUsername.Langue), session.Langue));
                return;
            }*/

            var BadgeCode = parameters[2];
            clientByUsername.GetUser().GetBadgeComponent().GiveBadge(BadgeCode, true);
            clientByUsername.SendPacket(new ReceiveBadgeComposer(BadgeCode));
        }
        else
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", session.Langue));
        }
    }
}
