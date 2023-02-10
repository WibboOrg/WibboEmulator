namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Bots;
using WibboEmulator.Games.Users.Inventory.Bots;

internal sealed class BotInventoryComposer : ServerPacket
{
    public BotInventoryComposer(ICollection<Bot> bots)
       : base(ServerPacketHeader.USER_BOTS)
    {
        this.WriteInteger(bots.Count);
        foreach (var bot in bots.ToList())
        {
            this.WriteInteger(bot.Id);
            this.WriteString(bot.Name);
            this.WriteString(bot.Motto);
            this.WriteString(bot.Gender);
            this.WriteString(bot.Figure);
        }
    }
}
