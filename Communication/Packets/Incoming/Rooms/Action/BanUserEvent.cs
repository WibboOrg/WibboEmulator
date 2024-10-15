namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Action;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class BanUserEvent : IPacketEvent
{
    private const int BAN_HOUR = 3600;
    private const int BAN_DAY = 86400;
    private const int BAN_PERMANENT = -1;

    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session.User == null)
        {
            return;
        }

        if (!RoomManager.TryGetRoom(Session.User.RoomId, out var room))
        {
            return;
        }

        if (!CanBanUser(Session, room))
        {
            return;
        }

        var userId = packet.PopInt();
        _ = packet.PopInt(); // This line seems to discard the next int from the packet
        var banType = packet.PopString();

        if (!TryGetBanDuration(banType, out var banDuration))
        {
            return;
        }

        var roomUserByUserId = room.RoomUserManager.GetRoomUserByUserId(userId);

        if (!IsValidBanTarget(room, roomUserByUserId))
        {
            return;
        }

        var expireTime = banDuration == -1 ? int.MaxValue : WibboEnvironment.GetUnixTimestamp() + banDuration;
        room.AddBan(userId, expireTime);
        room.RoomUserManager.RemoveUserFromRoom(roomUserByUserId.Client, true, true);
    }

    private static bool CanBanUser(GameClient Session, Room room) => (room.RoomData.BanFuse == 1 && room.CheckRights(Session)) || room.CheckRights(Session, true);

    private static bool TryGetBanDuration(string banType, out int duration) => banType switch
    {
        "RWUAM_BAN_USER_HOUR" => BanDuration(BAN_HOUR, out duration),
        "RWUAM_BAN_USER_DAY" => BanDuration(BAN_DAY, out duration),
        "RWUAM_BAN_USER_PERM" => BanDuration(BAN_PERMANENT, out duration),
        _ => BanDuration(0, out duration, false),
    };

    private static bool BanDuration(int duration, out int result, bool isValid = true)
    {
        result = duration;
        return isValid;
    }

    private static bool IsValidBanTarget(Room room, RoomUser roomUser)
    {
        if (roomUser == null || roomUser.IsBot)
        {
            return false;
        }

        var client = roomUser.Client;
        var user = client.User;

        return !room.CheckRights(client, true) &&
               !user.HasPermission("kick") &&
               !user.HasPermission("no_kick");
    }
}
