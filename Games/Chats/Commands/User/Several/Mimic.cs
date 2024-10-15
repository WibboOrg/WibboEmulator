namespace WibboEmulator.Games.Chats.Commands.User.Several;
using WibboEmulator.Communication.Packets.Outgoing.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class Mimic : IChatCommand
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

        var targetUser = GameClientManager.GetClientByUsername(username);
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

            if (targetUser.User.HasPremiumProtect && !session.User.HasPermission("mod"))
            {
                session.SendWhisper(LanguageManager.TryGetValue("premium.notallowed", session.Language));
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
