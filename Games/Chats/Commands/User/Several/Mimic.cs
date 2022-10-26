namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Communication.Packets.Outgoing.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Mimic : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (room.IsRoleplay && !room.CheckRights(session))
        {
            return;
        }

        if (parameters.Length != 2)
        {
            return;
        }

        var username = parameters[1];

        var targetUser = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(username);
        if (targetUser == null || targetUser.User == null)
        {
            var bot = room.RoomUserManager.GetBotByName(username);
            if (bot == null || bot.BotData == null)
            {
                return;
            }

            session.User.Gender = bot.BotData.Gender;
            session.User.Look = bot.BotData.Look;
        }
        else
        {

            if (targetUser.User.PremiumProtect && !session.User.HasPermission("perm_mod"))
            {
                session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("premium.notallowed", session.Langue));
                return;
            }

            session.User.Gender = targetUser.User.Gender;
            session.User.Look = targetUser.User.Look;
        }

        if (userRoom.IsTransf || userRoom.IsSpectator)
        {
            return;
        }

        session.SendPacket(new FigureUpdateComposer(session.User.Look, session.User.Gender));
        session.SendPacket(new UserChangeComposer(userRoom, true));
        room.SendPacket(new UserChangeComposer(userRoom, false));
    }
}
