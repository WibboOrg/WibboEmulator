namespace WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.Groups;

internal class UserGroupBadgesComposer : ServerPacket
{
    public UserGroupBadgesComposer(Dictionary<int, string> Badges)
        : base(ServerPacketHeader.GROUP_BADGES)
    {
        this.WriteInteger(Badges.Count);
        foreach (var Badge in Badges)
        {
            this.WriteInteger(Badge.Key);
            this.WriteString(Badge.Value);
        }
    }

    public UserGroupBadgesComposer(Group Group)
        : base(ServerPacketHeader.GROUP_BADGES)
    {
        this.WriteInteger(1);//count
        {
            this.WriteInteger(Group.Id);
            this.WriteString(Group.Badge);
        }
    }
}
