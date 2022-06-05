using Butterfly.Game.Clients;
using Butterfly.Game.Items;

namespace Butterfly.Game.Rooms.Trading
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
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(this._roomId);
            if (room == null)
            {
                return null;
            }
            else
            {
                return room.GetRoomUserManager().GetRoomUserByUserId(this.UserId);
            }
        }

        public Client GetClient()
        {
            return ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(this.UserId);
        }
    }
}