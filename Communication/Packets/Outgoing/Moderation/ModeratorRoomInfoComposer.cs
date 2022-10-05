namespace WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.Rooms;

internal class ModeratorRoomInfoComposer : ServerPacket
{
    public ModeratorRoomInfoComposer(RoomData data, bool ownerInRoom)
        : base(ServerPacketHeader.MODTOOL_ROOM_INFO)
    {
        this.WriteInteger(data.Id);
        this.WriteInteger(data.UsersNow);

        this.WriteBoolean(ownerInRoom);

        this.WriteInteger(data.OwnerId);
        this.WriteString(data.OwnerName);
        this.WriteBoolean(true);

        this.WriteString(data.Name);
        this.WriteString(data.Description);
        this.WriteInteger(data.TagCount);
        foreach (var s in data.Tags)
        {
            this.WriteString(s);
        }
    }
}
