using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Users;

namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings
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
                User Data = WibboEnvironment.GetUserById(Id);
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
