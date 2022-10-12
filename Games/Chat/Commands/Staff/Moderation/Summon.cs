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
        if (targetUser == null || targetUser.GetUser() == null)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("input.useroffline", session.Langue));
            return;
        }
        else if (targetUser.GetUser().CurrentRoom != null && targetUser.GetUser().CurrentRoom.Id == session.GetUser().CurrentRoom.Id)
        {
            return;
        }

        var currentRoom = session.GetUser().CurrentRoom;
        targetUser.GetUser().IsTeleporting = true;
        targetUser.GetUser().TeleportingRoomID = currentRoom.Data.Id;
        targetUser.GetUser().TeleporterId = 0;

        targetUser.SendPacket(new GetGuestRoomResultComposer(targetUser, currentRoom.Data, false, true));
    }
}
