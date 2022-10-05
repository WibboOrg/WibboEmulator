namespace WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Users;

internal class GroupMembersComposer : ServerPacket
{
    public GroupMembersComposer(Group group, ICollection<User> members, int membersCount, int page, bool admin, int reqType, string searchVal)
        : base(ServerPacketHeader.GROUP_MEMBERS)
    {
        this.WriteInteger(group.Id);
        this.WriteString(group.Name);
        this.WriteInteger(group.RoomId);
        this.WriteString(group.Badge);
        this.WriteInteger(membersCount);

        this.WriteInteger(members.Count);
        if (membersCount > 0)
        {
            foreach (var Data in members)
            {
                this.WriteInteger(group.CreatorId == Data.Id ? 0 : group.IsAdmin(Data.Id) ? 1 : group.IsMember(Data.Id) ? 2 : 3);
                this.WriteInteger(Data.Id);
                this.WriteString(Data.Username);
                this.WriteString(Data.Look);
                this.WriteString(string.Empty);
            }
        }
        this.WriteBoolean(admin);
        this.WriteInteger(14);
        this.WriteInteger(page);
        this.WriteInteger(reqType);
        this.WriteString(searchVal);
    }
}
