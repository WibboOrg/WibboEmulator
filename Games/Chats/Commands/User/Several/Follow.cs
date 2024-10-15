namespace WibboEmulator.Games.Chats.Commands.User.Several;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class Follow : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var TargetUser = GameClientManager.GetClientByUsername(parameters[1]);

        if (TargetUser == null || TargetUser.User == null)
        {
            Session.SendWhisper(LanguageManager.TryGetValue("input.useroffline", Session.Language));
        }
        else if (TargetUser.User.HideInRoom && !Session.User.HasPermission("mod"))
        {
            Session.SendWhisper(LanguageManager.TryGetValue("cmd.follow.notallowed", Session.Language));
        }
        else
        {
            var currentRoom = TargetUser.User.Room;
            if (currentRoom != null)
            {
                Session.SendPacket(new GetGuestRoomResultComposer(Session, currentRoom.RoomData, false, true));
            }
        }
    }
}
