namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Badges;
using WibboEmulator.Games.Users.Badges;

internal sealed class BadgesComposer : ServerPacket
{
    public BadgesComposer(Dictionary<string, Badge> badges)
        : base(ServerPacketHeader.USER_BADGES)
    {
        var list = new List<Badge>();
        var i = 0;

        this.WriteInteger(badges.Count);
        foreach (var badge in badges.Values)
        {
            this.WriteInteger(i++);
            this.WriteString(badge.Code);
            if (badge.Slot > 0)
            {
                list.Add(badge);
            }
        }

        this.WriteInteger(list.Count);
        foreach (var badge in list.OrderBy(x => x.Slot))
        {
            this.WriteInteger(badge.Slot);
            this.WriteString(badge.Code);
        }
    }
}
