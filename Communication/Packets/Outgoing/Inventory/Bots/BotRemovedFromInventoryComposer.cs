namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Bots;

internal class BotRemovedFromInventoryComposer : ServerPacket
{
    public BotRemovedFromInventoryComposer(int botId)
       : base(ServerPacketHeader.REMOVE_BOT_FROM_INVENTORY) => this.WriteInteger(botId);
}
