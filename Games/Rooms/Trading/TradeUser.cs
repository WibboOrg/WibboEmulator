namespace WibboEmulator.Games.Rooms.Trading;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

public class TradeUser
{
    public int UserId { get; set; }
    public List<Item> OfferedItems { get; set; }
    public bool HasAccepted { get; set; }

    private readonly int _roomId;

    public TradeUser(int userId, int roomId)
    {
        this._roomId = roomId;

        this.UserId = userId;
        this.HasAccepted = false;
        this.OfferedItems = new List<Item>();
    }

    public RoomUser GetRoomUser()
    {
        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(this._roomId, out var room))
        {
            return null;
        }

        return room.GetRoomUserManager().GetRoomUserByUserId(this.UserId);
    }

    public GameClient GetClient() => WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(this.UserId);
}
