using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using System.Collections.Generic;

namespace Butterfly.Game.Rooms
{
    public class TradeUser
    {
        public int UserId;
        private readonly int RoomId;
        public List<Item> OfferedItems;

        public bool HasAccepted { get; set; }

        public TradeUser(int UserId, int RoomId)
        {
            this.UserId = UserId;
            this.RoomId = RoomId;
            this.HasAccepted = false;
            this.OfferedItems = new List<Item>();
        }

        public RoomUser GetRoomUser()
        {
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(this.RoomId);
            if (room == null)
            {
                return null;
            }
            else
            {
                return room.GetRoomUserManager().GetRoomUserByHabboId(this.UserId);
            }
        }

        public Client GetClient()
        {
            return ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(this.UserId);
        }
    }
}