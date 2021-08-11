using Butterfly.HabboHotel.Rooms;
using Butterfly.HabboHotel.Users;

namespace Butterfly.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class GetRoomBannedUsersComposer : ServerPacket
    {
        public GetRoomBannedUsersComposer(Room instance)
            : base(ServerPacketHeader.ROOM_BAN_LIST)
        {
            this.WriteInteger(instance.Id);

            this.WriteInteger(instance.getBans().Count);//Count
            foreach (int Id in instance.getBans().Keys)
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
