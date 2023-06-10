namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Users;

internal sealed class Summon : IChatCommand
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

        if (!targetUser.User.IsTeleporting)
        {
            targetUser.User.IsTeleporting = true;
            targetUser.User.TeleportingRoomID = room.RoomData.Id;
            targetUser.User.TeleporterId = 0;

            targetUser.SendPacket(new GetGuestRoomResultComposer(targetUser, room.RoomData, false, true));
        }
    }
}
