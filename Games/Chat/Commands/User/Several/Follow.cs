namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Follow : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var targetUser = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(parameters[1]);

        if (targetUser == null || targetUser.GetUser() == null)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("input.useroffline", session.Langue));
        }
        else if (targetUser.GetUser().HideInRoom && !session.GetUser().HasPermission("perm_mod"))
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.follow.notallowed", session.Langue));
        }
        else
        {
            var currentRoom = targetUser.GetUser().CurrentRoom;
            if (currentRoom != null)
            {
                session.SendPacket(new GetGuestRoomResultComposer(session, currentRoom.RoomData, false, true));
            }
        }
    }
}
