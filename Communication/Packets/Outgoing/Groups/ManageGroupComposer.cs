using Butterfly.HabboHotel.Groups;

namespace Butterfly.Communication.Packets.Outgoing.Groups
{
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

            for (int x = 0; x < BadgeParts.Length; x++)
            {
                string symbol = BadgeParts[x];

                this.WriteInteger((symbol.Length >= 6) ? (int.TryParse(symbol.Substring(0, 3), out int symbolInt)) ? symbolInt : 0 : (int.TryParse(symbol.Substring(0, 2), out symbolInt)) ? symbolInt : 0);
                this.WriteInteger((symbol.Length >= 6) ? (int.TryParse(symbol.Substring(3, 2), out symbolInt)) ? symbolInt : 0 : (int.TryParse(symbol.Substring(2, 2), out symbolInt)) ? symbolInt : 0);
                this.WriteInteger(symbol.Length < 5 ? 0 : symbol.Length >= 6 ? (int.TryParse(symbol.Substring(5, 1), out symbolInt)) ? symbolInt : 0 : (int.TryParse(symbol.Substring(4, 1), out symbolInt)) ? symbolInt : 0);
            }

            int i = 0;
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
}
