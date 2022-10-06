namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class UserChangeComposer : ServerPacket
{
    public UserChangeComposer(RoomUser user, bool self)
        : base(ServerPacketHeader.UNIT_INFO)
    {
        this.WriteInteger(self ? -1 : user.VirtualId);
        this.WriteString(user.GetClient().GetUser().Look);
        this.WriteString(user.GetClient().GetUser().Gender);
        this.WriteString(user.GetClient().GetUser().Motto);
        this.WriteInteger(user.GetClient().GetUser().AchievementPoints);
    }

    public UserChangeComposer(RoomUser user) //Bot
        : base(ServerPacketHeader.UNIT_INFO)
    {
        this.WriteInteger(user.VirtualId);
        this.WriteString(user.BotData.Look);
        this.WriteString(user.BotData.Gender);
        this.WriteString(user.BotData.Motto);
        this.WriteInteger(0);
    }

    public UserChangeComposer(GameClient client)
        : base(ServerPacketHeader.UNIT_INFO)
    {
        this.WriteInteger(-1);
        this.WriteString(client.GetUser().Look);
        this.WriteString(client.GetUser().Gender);
        this.WriteString(client.GetUser().Motto);
        this.WriteInteger(client.GetUser().AchievementPoints);
    }
}
