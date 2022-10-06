namespace WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.Groups;

internal class UserGroupBadgesComposer : ServerPacket
{
    public UserGroupBadgesComposer(Dictionary<int, string> badges)
        : base(ServerPacketHeader.GROUP_BADGES)
    {
        this.WriteInteger(badges.Count);
        foreach (var badge in badges)
        {
            this.WriteInteger(badge.Key);
            this.WriteString(badge.Value);
        }
    }

    public UserGroupBadgesComposer(Group group)
        : base(ServerPacketHeader.GROUP_BADGES)
    {
        this.WriteInteger(1);//count
        {
            this.WriteInteger(group.Id);
            this.WriteString(group.Badge);
        }
    }
}
