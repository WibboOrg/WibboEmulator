namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;
using WibboEmulator.Games.Rooms;

internal class RoomRightsListComposer : ServerPacket
{
    public RoomRightsListComposer(Room Instance)
        : base(ServerPacketHeader.ROOM_RIGHTS_LIST)
    {
        this.WriteInteger(Instance.Id);

        this.WriteInteger(Instance.UsersWithRights.Count);
        foreach (var Id in Instance.UsersWithRights.ToList())
        {
            var Data = WibboEnvironment.GetUserById(Id);
            if (Data == null)
            {
                this.WriteInteger(0);
                this.WriteString("Unknown Error");
            }
            else
            {
                this.WriteInteger(Data.Id);
                this.WriteString(Data.Username);
            }
        }
    }
}
