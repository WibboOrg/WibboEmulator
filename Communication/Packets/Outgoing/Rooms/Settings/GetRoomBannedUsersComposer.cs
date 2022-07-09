using Wibbo.Game.Rooms;
using Wibbo.Game.Users;

namespace Wibbo.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class GetRoomBannedUsersComposer : ServerPacket
    {
        public GetRoomBannedUsersComposer(Room instance)
            : base(ServerPacketHeader.ROOM_BAN_LIST)
        {
            this.WriteInteger(instance.Id);

            this.WriteInteger(instance.GetBans().Count);//Count
            foreach (int Id in instance.GetBans().Keys)
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
