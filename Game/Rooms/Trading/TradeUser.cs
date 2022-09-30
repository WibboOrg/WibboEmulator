using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Items;

namespace WibboEmulator.Game.Rooms.Trading
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

        public Client GetClient()
        {
            return WibboEnvironment.GetGame().GetClientManager().GetClientByUserID(this.UserId);
        }
    }
}