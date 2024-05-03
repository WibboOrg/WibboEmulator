namespace WibboEmulator.Games.Rooms.Trading;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

public class TradeUser(int userId, int roomId)
{
    public int UserId { get; set; } = userId;
    public List<Item> OfferedItems { get; set; } = [];
    public bool HasAccepted { get; set; } = false;

    public RoomUser RoomUser
    {
        get
        {
            if (!RoomManager.TryGetRoom(roomId, out var room))
            {
                return null;
            }

            return room.RoomUserManager.GetRoomUserByUserId(this.UserId);
        }
    }

    public GameClient Client => GameClientManager.GetClientByUserID(this.UserId);
}
