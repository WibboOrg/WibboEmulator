namespace WibboEmulator.Games.Chats.Commands.User.Several;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class Follow : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var targetUser = GameClientManager.GetClientByUsername(parameters[1]);

        if (targetUser == null || targetUser.User == null)
        {
            session.SendWhisper(LanguageManager.TryGetValue("input.useroffline", session.Language));
        }
        else if (targetUser.User.HideInRoom && !session.User.HasPermission("mod"))
        {
            session.SendWhisper(LanguageManager.TryGetValue("cmd.follow.notallowed", session.Language));
        }
        else
        {
            var currentRoom = targetUser.User.Room;
            if (currentRoom != null)
            {
                session.SendPacket(new GetGuestRoomResultComposer(session, currentRoom.RoomData, false, true));
            }
        }
    }
}
