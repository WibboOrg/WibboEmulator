namespace WibboEmulator.Games.Chat.Commands.Staff.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Summon : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var targetUser = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(parameters[1]);
        if (targetUser == null || targetUser.User == null)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("input.useroffline", session.Langue));
            return;
        }
        else if (targetUser.User.CurrentRoom != null && targetUser.User.CurrentRoom.Id == session.User.CurrentRoom.Id)
        {
            return;
        }

        var currentRoom = session.User.CurrentRoom;
        targetUser.User.IsTeleporting = true;
        targetUser.User.TeleportingRoomID = currentRoom.RoomData.Id;
        targetUser.User.TeleporterId = 0;

        targetUser.SendPacket(new GetGuestRoomResultComposer(targetUser, currentRoom.RoomData, false, true));
    }
}
