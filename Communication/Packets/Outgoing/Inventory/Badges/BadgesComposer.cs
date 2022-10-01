using WibboEmulator.Games.Users.Badges;

namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Badges
{
    internal class BadgesComposer : ServerPacket
    {
        public BadgesComposer(Dictionary<string, Badge> badges)
            : base(ServerPacketHeader.USER_BADGES)
        {
            List<Badge> list = new List<Badge>();

            WriteInteger(badges.Count);
            foreach (Badge badge in badges.Values)
            {
                WriteInteger(0);
                WriteString(badge.Code);
                if (badge.Slot > 0)
                {
                    list.Add(badge);
                }
            }

            WriteInteger(list.Count);
            foreach (Badge badge in list)
            {
                WriteInteger(badge.Slot);
                WriteString(badge.Code);
            }
        }
    }
}
