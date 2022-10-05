namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Badges;
using WibboEmulator.Games.Users.Badges;

internal class BadgesComposer : ServerPacket
{
    public BadgesComposer(Dictionary<string, Badge> badges)
        : base(ServerPacketHeader.USER_BADGES)
    {
        var list = new List<Badge>();

        this.WriteInteger(badges.Count);
        foreach (var badge in badges.Values)
        {
            this.WriteInteger(0);
            this.WriteString(badge.Code);
            if (badge.Slot > 0)
            {
                list.Add(badge);
            }
        }

        this.WriteInteger(list.Count);
        foreach (var badge in list)
        {
            this.WriteInteger(badge.Slot);
            this.WriteString(badge.Code);
        }
    }
}
