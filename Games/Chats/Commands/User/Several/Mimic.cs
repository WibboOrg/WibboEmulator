namespace WibboEmulator.Games.Chats.Commands.User.Several;
using WibboEmulator.Communication.Packets.Outgoing.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class Mimic : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (room.IsRoleplay && !room.CheckRights(Session))
        {
            return;
        }

        if (parameters.Length != 2)
        {
            return;
        }

        var username = parameters[1];

        var TargetUser = GameClientManager.GetClientByUsername(username);
        if (TargetUser == null || TargetUser.User == null)
        {
            var bot = room.RoomUserManager.GetBotByName(username);
            if (bot == null || bot.BotData == null)
            {
                return;
            }

            Session.User.Gender = bot.BotData.Gender;
            Session.User.Look = bot.BotData.Look;
        }
        else
        {

            if (TargetUser.User.HasPremiumProtect && !Session.User.HasPermission("mod"))
            {
                Session.SendWhisper(LanguageManager.TryGetValue("premium.notallowed", Session.Language));
                return;
            }

            Session.User.Gender = TargetUser.User.Gender;
            Session.User.Look = TargetUser.User.Look;
        }

        if (userRoom.IsTransf || userRoom.IsSpectator)
        {
            return;
        }

        Session.SendPacket(new FigureUpdateComposer(Session.User.Look, Session.User.Gender));
        Session.SendPacket(new UserChangeComposer(userRoom, true));
        room.SendPacket(new UserChangeComposer(userRoom, false));
    }
}
