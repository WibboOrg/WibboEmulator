namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Items.Wired;
using WibboEmulator.Games.Rooms;

internal class ObjectsComposer : ServerPacket
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
            this.WriteFloorItem(item, Convert.ToInt32(room.RoomData.OwnerId), room.RoomData.HideWireds);
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
            this.WriteFloorItem(item, Convert.ToInt32(room.RoomData.OwnerId));
        }
    }

    private void WriteFloorItem(ItemTemp item, int userId)
    {
        this.WriteInteger(item.Id);
        this.WriteInteger(item.SpriteId);
        this.WriteInteger(item.X);
        this.WriteInteger(item.Y);
        this.WriteInteger(2);
        this.WriteString(string.Format(/*lang=json*/ "{0:0.00}", item.Z));
        this.WriteString(string.Empty);

        if (item.InteractionType == InteractionTypeTemp.RPITEM)
        {
            this.WriteInteger(0);
            this.WriteInteger(1);

            this.WriteInteger(5);

            this.WriteString("state");
            this.WriteString("0");
            this.WriteString("imageUrl");
            this.WriteString("https://swf.wibbo.me/items/" + item.ExtraData + ".png");
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

        this.WriteInteger(-1); // to-do: check
        this.WriteInteger(1); //(Item.GetBaseItem().Modes > 1) ? 1 : 0
        this.WriteInteger(userId);
    }

    private void WriteFloorItem(Item item, int userId, bool hideWired)
    {

        this.WriteInteger(item.Id);
        this.WriteInteger((hideWired && WiredUtillity.TypeIsWired(item.GetBaseItem().InteractionType) && item.GetBaseItem().InteractionType != InteractionType.HIGHSCORE && item.GetBaseItem().InteractionType != InteractionType.HIGHSCOREPOINTS) ? 31294061 : item.GetBaseItem().SpriteId);
        this.WriteInteger(item.X);
        this.WriteInteger(item.Y);
        this.WriteInteger(item.Rotation);
        this.WriteString(string.Format(/*lang=json*/ "{0:0.00}", item.Z));
        this.WriteString(item.GetBaseItem().Height.ToString());

        ItemBehaviourUtility.GenerateExtradata(item, this);

        this.WriteInteger(-1); // expires
        this.WriteInteger(2); //(Item.GetBaseItem().Modes > 1) ? 1 : 0
        this.WriteInteger(userId);
    }
}
