using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class Follow : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            GameClient TargetUser = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(Params[1]);

            if (TargetUser == null || TargetUser.GetUser() == null)
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("input.useroffline", Session.Langue));
            }
            else if ((TargetUser.GetUser().HideInRoom) && !Session.GetUser().HasPermission("perm_mod"))
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.follow.notallowed", Session.Langue));
            }
            else if (TargetUser.GetUser().Rank >= 8)
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.follow.notallowed", Session.Langue));
            }
            else
            {
                Room currentRoom = TargetUser.GetUser().CurrentRoom;
                if (currentRoom != null)
                {
                    Session.SendPacket(new GetGuestRoomResultComposer(Session, currentRoom.RoomData, false, true));
                }
            }
        }
    }
}
