namespace WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Games.Groups;

internal sealed class GroupFurniConfigComposer : ServerPacket
{
    public GroupFurniConfigComposer(ICollection<Group> groups)
        : base(ServerPacketHeader.GROUP_LIST)
    {
        this.WriteInteger(groups.Count);
        foreach (var group in groups)
        {
            this.WriteInteger(group.Id);
            this.WriteString(group.Name);
            this.WriteString(group.Badge);
            this.WriteString(WibboEnvironment.GetGame().GetGroupManager().GetColourCode(group.Colour1, true));
            this.WriteString(WibboEnvironment.GetGame().GetGroupManager().GetColourCode(group.Colour2, false));
            this.WriteBoolean(false);
            this.WriteInteger(group.CreatorId);
            this.WriteBoolean(group.ForumEnabled);
        }
    }
}
