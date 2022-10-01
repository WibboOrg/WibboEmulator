using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class UserChangeComposer : ServerPacket
    {
        public UserChangeComposer(RoomUser user, bool self)
            : base(ServerPacketHeader.UNIT_INFO)
        {
            this.WriteInteger((self) ? -1 : user.VirtualId);
            this.WriteString(user.GetClient().GetUser().Look);
            this.WriteString(user.GetClient().GetUser().Gender);
            this.WriteString(user.GetClient().GetUser().Motto);
            this.WriteInteger(user.GetClient().GetUser().AchievementPoints);
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
            this.WriteString(Client.GetUser().Look);
            this.WriteString(Client.GetUser().Gender);
            this.WriteString(Client.GetUser().Motto);
            this.WriteInteger(Client.GetUser().AchievementPoints);
        }
    }
}
