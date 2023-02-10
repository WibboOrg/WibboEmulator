namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.Items;

internal sealed class ObjectAddComposer : ServerPacket
{
    public ObjectAddComposer(Item item, string userame, int userId)
        : base(ServerPacketHeader.FURNITURE_FLOOR_ADD)
    {
        this.WriteInteger(item.Id);
        this.WriteInteger(item.GetBaseItem().SpriteId);
        this.WriteInteger(item.X);
        this.WriteInteger(item.Y);
        this.WriteInteger(item.Rotation);
        this.WriteString(string.Format(/*lang=json*/ "{0:0.00}", item.Z));
        this.WriteString(string.Empty);

        ItemBehaviourUtility.GenerateExtradata(item, this);

        this.WriteInteger(-1);
        this.WriteInteger(1);
        this.WriteInteger(userId);
        this.WriteString(userame);
    }

    public ObjectAddComposer(ItemTemp item)
        : base(ServerPacketHeader.FURNITURE_FLOOR_ADD)
    {
        this.WriteInteger(item.Id);
        this.WriteInteger(item.SpriteId);
        this.WriteInteger(item.X);
        this.WriteInteger(item.Y);
        this.WriteInteger(2);
        this.WriteString(string.Format(/*lang=json*/ "{0:0.00}", item.Z));
        this.WriteString("");

        if (item.InteractionType == InteractionTypeTemp.RpItem)
        {
            this.WriteInteger(0);
            this.WriteInteger(1);

            this.WriteInteger(5);

            this.WriteString("state");
            this.WriteString("0");
            this.WriteString("imageUrl");
            this.WriteString("https://cdn.wibbo.org/items/" + item.ExtraData + ".png");
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
            this.WriteString(item.ExtraData); //ExtraData
        }


        this.WriteInteger(-1); // to-do: check
        this.WriteInteger(1);
        this.WriteInteger(item.VirtualUserId);
        this.WriteString("");
    }
}
