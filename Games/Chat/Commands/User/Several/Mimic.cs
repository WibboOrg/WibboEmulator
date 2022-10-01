using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Communication.Packets.Outgoing.Avatar;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class Mimic : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Room.IsRoleplay && !Room.CheckRights(Session))
            {
                return;
            }

            if (Params.Length != 2)
            {
                return;
            }

            string Username = Params[1];

            GameClient TargetUser = WibboEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetUser == null || TargetUser.GetUser() == null)
            {
                RoomUser Bot = Room.GetRoomUserManager().GetBotByName(Username);
                if (Bot == null || Bot.BotData == null)
                {
                    return;
                }

                Session.GetUser().Gender = Bot.BotData.Gender;
                Session.GetUser().Look = Bot.BotData.Look;
            }
            else
            {

                if (TargetUser.GetUser().PremiumProtect && !Session.GetUser().HasPermission("perm_mod"))
                {
                    Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("premium.notallowed", Session.Langue));
                    return;
                }

                Session.GetUser().Gender = TargetUser.GetUser().Gender;
                Session.GetUser().Look = TargetUser.GetUser().Look;
            }

            if (UserRoom.IsTransf || UserRoom.IsSpectator)
            {
                return;
            }

            Session.SendPacket(new FigureUpdateComposer(Session.GetUser().Look, Session.GetUser().Gender));
            Session.SendPacket(new UserChangeComposer(UserRoom, true));
            Room.SendPacket(new UserChangeComposer(UserRoom, false));
        }
    }
}
