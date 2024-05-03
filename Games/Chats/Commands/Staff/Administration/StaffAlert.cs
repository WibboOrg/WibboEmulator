namespace WibboEmulator.Games.Chats.Commands.Staff.Administration;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class StaffAlert : IChatCommand
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

        foreach (var staff in GameClientManager.Clients.ToList())
        {
            if (staff == null)
            {
                continue;
            }

            if (staff.User == null)
            {
                continue;
            }

            if (staff.User.Room == null)
            {
                continue;
            }

            if (staff.User.Rank < 3)
            {
                continue;
            }

            var user = staff.User.Room.RoomUserManager.GetRoomUserByUserId(staff.User.Id);

            if (user == null)
            {
                continue;
            }

            user.Client?.SendPacket(new WhisperComposer(user.VirtualId, "[STAFF ALERT] " + messageTxt + " - " + roomUser.Username, 23));
        }
    }
}
