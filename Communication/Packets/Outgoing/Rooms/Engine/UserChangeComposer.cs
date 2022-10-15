namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class UserChangeComposer : ServerPacket
{
    public UserChangeComposer(RoomUser user, bool self)
        : base(ServerPacketHeader.UNIT_INFO)
    {
        this.WriteInteger(self ? -1 : user.VirtualId);
        this.WriteString(user.Client.User.Look);
        this.WriteString(user.Client.User.Gender);
        this.WriteString(user.Client.User.Motto);
        this.WriteInteger(user.Client.User.AchievementPoints);
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
        this.WriteString(client.User.Look);
        this.WriteString(client.User.Gender);
        this.WriteString(client.User.Motto);
        this.WriteInteger(client.User.AchievementPoints);
    }
}
