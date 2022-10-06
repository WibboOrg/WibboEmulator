namespace WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Games.Groups;

internal class ManageGroupComposer : ServerPacket
{
    public ManageGroupComposer(Group Group, string[] BadgeParts)
        : base(ServerPacketHeader.GROUP_SETTINGS)
    {
        this.WriteInteger(0);
        this.WriteBoolean(true);
        this.WriteInteger(Group.Id);
        this.WriteString(Group.Name);
        this.WriteString(Group.Description);
        this.WriteInteger(1);
        this.WriteInteger(Group.Colour1);
        this.WriteInteger(Group.Colour2);
        this.WriteInteger(Group.GroupType == GroupType.OPEN ? 0 : Group.GroupType == GroupType.LOCKED ? 1 : 2);
        this.WriteInteger(Group.AdminOnlyDeco);
        this.WriteBoolean(false);
        this.WriteString("");

        this.WriteInteger(5);

        for (var x = 0; x < BadgeParts.Length; x++)
        {
            var symbol = BadgeParts[x];

            this.WriteInteger((symbol.Length >= 6) ? int.TryParse(symbol[..3], out var symbolInt) ? symbolInt : 0 : int.TryParse(symbol[..2], out symbolInt) ? symbolInt : 0);
            this.WriteInteger((symbol.Length >= 6) ? int.TryParse(symbol.AsSpan(3, 2), out symbolInt) ? symbolInt : 0 : int.TryParse(symbol.AsSpan(2, 2), out symbolInt) ? symbolInt : 0);
            this.WriteInteger(symbol.Length < 5 ? 0 : symbol.Length >= 6 ? int.TryParse(symbol.AsSpan(5, 1), out symbolInt) ? symbolInt : 0 : int.TryParse(symbol.AsSpan(4, 1), out symbolInt) ? symbolInt : 0);
        }

        var i = 0;
        while (i < (5 - BadgeParts.Length))
        {
            this.WriteInteger(0);
            this.WriteInteger(0);
            this.WriteInteger(0);
            i++;
        }

        this.WriteString(Group.Badge);
        this.WriteInteger(Group.MemberCount);
    }
}
