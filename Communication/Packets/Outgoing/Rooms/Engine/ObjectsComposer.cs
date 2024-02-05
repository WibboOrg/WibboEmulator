namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Items.Wired;
using WibboEmulator.Games.Rooms;

internal sealed class ObjectsComposer : ServerPacket
{
    public ObjectsComposer(Item[] items, Room room)
        : base(ServerPacketHeader.FURNITURE_FLOOR)
    {
        this.WriteInteger(1);

        this.WriteInteger(room.RoomData.OwnerId);
        this.WriteString(room.RoomData.OwnerName);

        this.WriteInteger(items.Length);
        foreach (var item in items)
        {
            this.WriteFloorItem(item, room.RoomData.HideWireds);

            this.WriteInteger(-1); // expires
            this.WriteInteger(room.IsGameMode ? 0 : 2);
            this.WriteInteger(room.RoomData.OwnerId);
        }
    }

    public ObjectsComposer(ItemTemp[] items, Room room)
        : base(ServerPacketHeader.FURNITURE_FLOOR)
    {
        this.WriteInteger(1);

        this.WriteInteger(room.RoomData.OwnerId);
        this.WriteString(room.RoomData.OwnerName);

        this.WriteInteger(items.Length);
        foreach (var item in items)
        {
            this.WriteFloorItem(item);

            this.WriteInteger(-1); // expires
            this.WriteInteger(room.IsGameMode ? 0 : 2);
            this.WriteInteger(room.RoomData.OwnerId);
        }
    }

    private void WriteFloorItem(ItemTemp item)
    {
        this.WriteInteger(item.Id);
        this.WriteInteger(item.SpriteId);
        this.WriteInteger(item.X);
        this.WriteInteger(item.Y);
        this.WriteInteger(2);
        this.WriteString(string.Format(/*lang=json*/ "{0:0.00}", item.Z));
        this.WriteString(string.Empty);

        if (item.InteractionType == InteractionTypeTemp.RpItem)
        {
            this.WriteInteger(0);
            this.WriteInteger(1);

            this.WriteInteger(5);

            this.WriteString("state");
            this.WriteString("0");
            this.WriteString("imageUrl");
            this.WriteString("https://" + WibboEnvironment.GetSettings().GetData<string>("cdn.url") + "/items/" + item.ExtraData + ".png");
            this.WriteString("offsetX");
            this.WriteString("-20");
            this.WriteString("offsetY");
            this.WriteString("10");
            this.WriteString("offsetZ");
            this.WriteString("10002");
        }
        else
        {
            this.WriteInteger(1);
            this.WriteInteger(0);
            this.WriteString(item.ExtraData);
        }
    }

    private void WriteFloorItem(Item item, bool hideWired)
    {
        this.WriteInteger(item.Id);
        this.WriteInteger(hideWired && WiredUtillity.AllowHideWiredType(item.GetBaseItem().InteractionType) ? WibboEnvironment.GetSettings().GetData<int>("wired.hide.item.id") : item.GetBaseItem().SpriteId);
        this.WriteInteger(item.X);
        this.WriteInteger(item.Y);
        this.WriteInteger(item.Rotation);
        this.WriteString(string.Format(/*lang=json*/ "{0:0.00}", item.Z));
        this.WriteString(item.Data.Height.ToString());
        this.WriteInteger(item.Extra);

        ItemBehaviourUtility.GenerateExtradata(item, this);
    }
}
