using Butterfly.Game.Rooms;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Outgoing.Navigator
{
    internal class FavouritesComposer : ServerPacket
    {
        public FavouritesComposer(List<RoomData> favouriteIDs)
            : base(ServerPacketHeader.USER_FAVORITE_ROOM_COUNT)
        {
            this.WriteInteger(30);
            this.WriteInteger(favouriteIDs.Count);

            foreach (RoomData Room in favouriteIDs)
            {
                this.WriteInteger(Room.Id);
            }
        }
    }
}
