namespace WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Games.Groups;

internal sealed class ManageGroupComposer : ServerPacket
{
    public ManageGroupComposer(Group group, string[] badgeParts)
        : base(ServerPacketHeader.GROUP_SETTINGS)
    {
        this.WriteInteger(0);
        this.WriteBoolean(true);
        this.WriteInteger(group.Id);
        this.WriteString(group.Name);
        this.WriteString(group.Description);
        this.WriteInteger(1);
        this.WriteInteger(group.Colour1);
        this.WriteInteger(group.Colour2);
        this.WriteInteger(group.GroupType == GroupType.Open ? 0 : group.GroupType == GroupType.Locked ? 1 : 2);
        this.WriteInteger(group.AdminOnlyDeco ? 1 : 0);
        this.WriteBoolean(false);
        this.WriteString("");

        this.WriteInteger(5);

        for (var x = 0; x < badgeParts.Length; x++)
        {
            var symbol = badgeParts[x];

            this.WriteInteger((symbol.Length >= 6) ? int.TryParse(symbol[..3], out var symbolInt) ? symbolInt : 0 : int.TryParse(symbol[..2], out symbolInt) ? symbolInt : 0);
            this.WriteInteger((symbol.Length >= 6) ? int.TryParse(symbol.AsSpan(3, 2), out symbolInt) ? symbolInt : 0 : int.TryParse(symbol.AsSpan(2, 2), out symbolInt) ? symbolInt : 0);
            this.WriteInteger(symbol.Length < 5 ? 0 : symbol.Length >= 6 ? int.TryParse(symbol.AsSpan(5, 1), out symbolInt) ? symbolInt : 0 : int.TryParse(symbol.AsSpan(4, 1), out symbolInt) ? symbolInt : 0);
        }

        var i = 0;
        while (i < (5 - badgeParts.Length))
        {
            this.WriteInteger(0);
            this.WriteInteger(0);
            this.WriteInteger(0);
            i++;
        }

        this.WriteString(group.Badge);
        this.WriteInteger(group.MemberCount);
    }
}
