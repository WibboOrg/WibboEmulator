namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;
using WibboEmulator.Games.Rooms;

internal class RoomRightsListComposer : ServerPacket
{
    public RoomRightsListComposer(Room instance)
        : base(ServerPacketHeader.ROOM_RIGHTS_LIST)
    {
        this.WriteInteger(instance.Id);

        this.WriteInteger(instance.UsersWithRights.Count);
        foreach (var id in instance.UsersWithRights.ToList())
        {
            var data = WibboEnvironment.GetUserById(id);
            if (data == null)
            {
                this.WriteInteger(0);
                this.WriteString("Unknown Error");
            }
            else
            {
                this.WriteInteger(data.Id);
                this.WriteString(data.Username);
            }
        }
    }
}
