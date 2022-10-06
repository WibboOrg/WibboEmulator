namespace WibboEmulator.Games.Rooms;

public static class RoomAccessUtility
{
    public static int GetRoomAccessPacketNum(RoomAccess access) => access switch
    {
        RoomAccess.Doorbell => 1,
        RoomAccess.Password => 2,
        RoomAccess.Invisible => 3,
        _ => 0,
    };

    public static RoomAccess ToRoomAccess(string id) => id switch
    {
        "locked" => RoomAccess.Doorbell,
        "password" => RoomAccess.Password,
        "invisible" => RoomAccess.Invisible,
        _ => RoomAccess.Open,
    };

    public static RoomAccess ToRoomAccess(int id) => id switch
    {
        1 => RoomAccess.Doorbell,
        2 => RoomAccess.Password,
        3 => RoomAccess.Invisible,
        _ => RoomAccess.Open,
    };
}
