namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Furni;
using WibboEmulator.Games.Items;

internal class OpenGiftComposer : ServerPacket
{
    public OpenGiftComposer(ItemData Data, string Text, Item Item, bool ItemIsInRoom)
        : base(ServerPacketHeader.GIFT_OPENED)
    {
        this.WriteString(Data.Type.ToString());
        this.WriteInteger(Data.SpriteId);
        this.WriteString(Data.ItemName);
        this.WriteInteger(Item.Id);
        this.WriteString(Data.Type.ToString());
        this.WriteBoolean(ItemIsInRoom);//Is it in the room?
        this.WriteString(Text);
    }
}