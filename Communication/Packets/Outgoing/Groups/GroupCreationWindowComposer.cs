namespace WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Games.Rooms;

internal sealed class GroupCreationWindowComposer : ServerPacket
{
    public GroupCreationWindowComposer(ICollection<RoomData> rooms)
        : base(ServerPacketHeader.GROUP_CREATE_OPTIONS)
    {
        this.WriteInteger(20);//Price

        this.WriteInteger(rooms.Count);//Room count that the user has.
        foreach (var room in rooms)
        {
            this.WriteInteger(room.Id);//Room Id
            this.WriteString(room.Name);//Room Name
            this.WriteBoolean(false);//What?
        }

        this.WriteInteger(5);
        this.WriteInteger(5);
        this.WriteInteger(11);
        this.WriteInteger(4);

        this.WriteInteger(6);
        this.WriteInteger(11);
        this.WriteInteger(4);

        this.WriteInteger(0);
        this.WriteInteger(0);
        this.WriteInteger(0);

        this.WriteInteger(0);
        this.WriteInteger(0);
        this.WriteInteger(0);

        this.WriteInteger(0);
        this.WriteInteger(0);
        this.WriteInteger(0);
    }
}
