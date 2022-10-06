namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Communication.Packets.Outgoing.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Mimic : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (Room.IsRoleplay && !Room.CheckRights(session))
        {
            return;
        }

        if (parameters.Length != 2)
        {
            return;
        }

        var Username = parameters[1];

        var TargetUser = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(Username);
        if (TargetUser == null || TargetUser.GetUser() == null)
        {
            var Bot = Room.GetRoomUserManager().GetBotByName(Username);
            if (Bot == null || Bot.BotData == null)
            {
                return;
            }

            session.GetUser().Gender = Bot.BotData.Gender;
            session.GetUser().Look = Bot.BotData.Look;
        }
        else
        {

            if (TargetUser.GetUser().PremiumProtect && !session.GetUser().HasPermission("perm_mod"))
            {
                session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("premium.notallowed", session.Langue));
                return;
            }

            session.GetUser().Gender = TargetUser.GetUser().Gender;
            session.GetUser().Look = TargetUser.GetUser().Look;
        }

        if (UserRoom.IsTransf || UserRoom.IsSpectator)
        {
            return;
        }

        session.SendPacket(new FigureUpdateComposer(session.GetUser().Look, session.GetUser().Gender));
        session.SendPacket(new UserChangeComposer(UserRoom, true));
        Room.SendPacket(new UserChangeComposer(UserRoom, false));
    }
}
