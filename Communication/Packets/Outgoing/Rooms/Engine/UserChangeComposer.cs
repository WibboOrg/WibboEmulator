using Butterfly.Game.GameClients;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class UserChangeComposer : ServerPacket
    {
        public UserChangeComposer(RoomUser User, bool Self)
            : base(ServerPacketHeader.UNIT_INFO)
        {
            this.WriteInteger((Self) ? -1 : User.VirtualId);
            this.WriteString(User.GetClient().GetHabbo().Look);
            this.WriteString(User.GetClient().GetHabbo().Gender);
            this.WriteString(User.GetClient().GetHabbo().Motto);
            this.WriteInteger(User.GetClient().GetHabbo().AchievementPoints);
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

        public UserChangeComposer(GameClient Client)
            : base(ServerPacketHeader.UNIT_INFO)
        {
            this.WriteInteger(-1);
            this.WriteString(Client.GetHabbo().Look);
            this.WriteString(Client.GetHabbo().Gender);
            this.WriteString(Client.GetHabbo().Motto);
            this.WriteInteger(Client.GetHabbo().AchievementPoints);
        }
    }
}
