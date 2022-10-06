namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;
using WibboEmulator.Games.Rooms;

internal class GetRoomBannedUsersComposer : ServerPacket
{
    public GetRoomBannedUsersComposer(Room instance)
        : base(ServerPacketHeader.ROOM_BAN_LIST)
    {
        this.WriteInteger(instance.Id);

        this.WriteInteger(instance.GetBans().Count);//Count
        foreach (var id in instance.GetBans().Keys)
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
