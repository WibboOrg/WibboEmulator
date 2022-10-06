namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Furni;
using WibboEmulator.Games.Items;

internal class OpenGiftComposer : ServerPacket
{
    public OpenGiftComposer(ItemData data, string text, Item item, bool itemIsInRoom)
        : base(ServerPacketHeader.GIFT_OPENED)
    {
        this.WriteString(data.Type.ToString());
        this.WriteInteger(data.SpriteId);
        this.WriteString(data.ItemName);
        this.WriteInteger(item.Id);
        this.WriteString(data.Type.ToString());
        this.WriteBoolean(itemIsInRoom);//Is it in the room?
        this.WriteString(text);
    }
}
