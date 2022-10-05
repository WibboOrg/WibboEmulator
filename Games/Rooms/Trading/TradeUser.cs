using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

namespace WibboEmulator.Games.Rooms.Trading
{
    public class TradeUser
    {
        public int UserId { get; set; }
        public List<Item> OfferedItems { get; set; }
        public bool HasAccepted { get; set; }

        private readonly int _roomId;

        public TradeUser(int UserId, int RoomId)
        {
            this._roomId = RoomId;

            this.UserId = UserId;
            this.HasAccepted = false;
            this.OfferedItems = new List<Item>();
        }

        public RoomUser GetRoomUser()
        {
            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(this._roomId, out Room room))
                return null;

            return room.GetRoomUserManager().GetRoomUserByUserId(this.UserId);
        }

        public GameClient GetClient() => WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(this.UserId);
    }
}