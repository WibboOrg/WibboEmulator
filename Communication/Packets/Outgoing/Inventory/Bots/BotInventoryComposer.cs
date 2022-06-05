using Wibbo.Game.Users.Inventory.Bots;

namespace Wibbo.Communication.Packets.Outgoing.Inventory.Bots
{
    internal class BotInventoryComposer : ServerPacket
    {
        public BotInventoryComposer(ICollection<Bot> Bots)
           : base(ServerPacketHeader.USER_BOTS)
        {
            this.WriteInteger(Bots.Count);
            foreach (Bot Bot in Bots.ToList())
            {
                this.WriteInteger(Bot.Id);
                this.WriteString(Bot.Name);
                this.WriteString(Bot.Motto);
                this.WriteString(Bot.Gender);
                this.WriteString(Bot.Figure);
            }
        }
    }
}
