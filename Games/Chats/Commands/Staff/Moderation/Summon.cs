namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class Summon : IChatCommand
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
            return;
        }
        else if (TargetUser.User.Room != null && TargetUser.User.Room.Id == Session.User.Room.Id)
        {
            return;
        }

        if (!TargetUser.User.IsTeleporting)
        {
            TargetUser.User.IsTeleporting = true;
            TargetUser.User.TeleportingRoomID = room.RoomData.Id;
            TargetUser.User.TeleporterId = 0;

            TargetUser.SendPacket(new GetGuestRoomResultComposer(TargetUser, room.RoomData, false, true));
        }
    }
}
