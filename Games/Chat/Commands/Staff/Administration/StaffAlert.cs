namespace WibboEmulator.Games.Chat.Commands.Staff.Administration;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class StaffAlert : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser roomUser, string[] parameters)
    {
        if (parameters.Length < 2)
        {
            return;
        }

        var messageTxt = CommandManager.MergeParams(parameters, 1);

        if (string.IsNullOrEmpty(messageTxt))
        {
            return;
        }

        foreach (var staff in WibboEnvironment.GetGame().GetGameClientManager().GetClients)
        {
            if (staff == null)
            {
                continue;
            }

            if (staff.User == null)
            {
                continue;
            }

            if (staff.User.CurrentRoom == null)
            {
                continue;
            }

            if (staff.User.Rank < 3)
            {
                continue;
            }

            var user = staff.User.CurrentRoom.RoomUserManager.GetRoomUserByUserId(staff.User.Id);

            user.Client.SendPacket(new WhisperComposer(user.VirtualId, "[STAFF ALERT] " + messageTxt + " - " + roomUser.GetUsername(), 23));
        }
    }
}
