using Butterfly.HabboHotel.Rooms;
using Butterfly.HabboHotel.Users;
using System.Linq;

namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class RoomRightsListComposer : ServerPacket
    {
        public RoomRightsListComposer(Room Instance)
            : base(ServerPacketHeader.ROOM_RIGHTS_LIST)
        {
            this.WriteInteger(Instance.Id);

            this.WriteInteger(Instance.UsersWithRights.Count);
            foreach (int Id in Instance.UsersWithRights.ToList())
            {
                Habbo Data = ButterflyEnvironment.GetHabboById(Id);
                if (Data == null)
                {
                    this.WriteInteger(0);
                    this.WriteString("Unknown Error");
                }
                else
                {
                    this.WriteInteger(Data.Id);
                    this.WriteString(Data.Username);
                }
            }
        }
    }
}
