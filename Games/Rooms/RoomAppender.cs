namespace WibboEmulator.Games.Rooms;
using WibboEmulator.Communication.Packets.Outgoing;
using WibboEmulator.Games.Navigators;

internal static class RoomAppender
{
    public static void WriteRoom(ServerPacket packet, RoomData data)
    {
        packet.WriteInteger(data.Id);
        packet.WriteString(data.Name);
        packet.WriteInteger(data.OwnerId);
        packet.WriteString(data.OwnerName);
        packet.WriteInteger((int)data.Access);
        packet.WriteInteger(data.UsersNow);
        packet.WriteInteger(data.UsersMax);
        packet.WriteString(data.Description);
        packet.WriteInteger(data.TrocStatus);
        packet.WriteInteger(data.Score);
        packet.WriteInteger(1);//Top rated room rank.
        packet.WriteInteger(data.Category);

        packet.WriteInteger(data.Tags.Count);
        foreach (var tag in data.Tags)
        {
            packet.WriteString(tag);
        }

        var roomType = 8;
        if (data.Group != null)
        {
            roomType += 2;
        }

        if (NavigatorManager.TryGetFeaturedRoom(data.Id, out var item))
        {
            roomType += 1;
        }

        packet.WriteInteger(roomType);

        if (item != null)
        {
            packet.WriteString(item.Image);
        }

        if (data.Group != null)
        {
            packet.WriteInteger(data.Group == null ? 0 : data.Group.Id);
            packet.WriteString(data.Group == null ? "" : data.Group.Name);
            packet.WriteString(data.Group == null ? "" : data.Group.Badge);
        }
    }
}
