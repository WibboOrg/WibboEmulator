using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class UserChangeComposer : ServerPacket
    {
        public UserChangeComposer(RoomUser User, bool Self)
            : base(ServerPacketHeader.UNIT_INFO)
        {
            this.WriteInteger((Self) ? -1 : User.VirtualId);
            this.WriteString(User.GetClient().GetUser().Look);
            this.WriteString(User.GetClient().GetUser().Gender);
            this.WriteString(User.GetClient().GetUser().Motto);
            this.WriteInteger(User.GetClient().GetUser().AchievementPoints);
        }

        public UserChangeComposer(RoomUser User) //Bot
            : base(ServerPacketHeader.UNIT_INFO)
        {
            this.WriteInteger(User.VirtualId);
            this.WriteString(User.BotData.Look);
            this.WriteString(User.BotData.Gender);
            this.WriteString(User.BotData.Motto);
            this.WriteInteger(0);
        }

        public UserChangeComposer(Client Client)
            : base(ServerPacketHeader.UNIT_INFO)
        {
            this.WriteInteger(-1);
            this.WriteString(Client.GetUser().Look);
            this.WriteString(Client.GetUser().Gender);
            this.WriteString(Client.GetUser().Motto);
            this.WriteInteger(Client.GetUser().AchievementPoints);
        }
    }
}
