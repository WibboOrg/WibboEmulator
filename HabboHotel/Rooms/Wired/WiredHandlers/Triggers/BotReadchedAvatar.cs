using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Triggers
{
    public class BotReadchedAvatar : IWired
    {
        private Item item;
        private WiredHandler handler;
        private readonly BotCollisionDelegate delegateFunction;
        private bool disposed;

        private string NameBot;

        public BotReadchedAvatar(Item ThisWired, WiredHandler handler, string pNameBot)
        {
            this.item = ThisWired;
            this.handler = handler;
            this.NameBot = pNameBot;
            this.delegateFunction = new BotCollisionDelegate(this.Collision);
            this.handler.TrgBotCollision += this.delegateFunction;
        }

        private void Collision(RoomUser user, string pBotName)
        {
            if (user == null || user.IsBot)
            {
                return;
            }

            if (!string.IsNullOrEmpty(this.NameBot) && this.NameBot != pBotName)
            {
                return;
            }

            this.handler.ExecutePile(this.item.Coordinate, user, null);
        }

        public bool Disposed()
        {
            return this.disposed;
        }

        public void Dispose()
        {
            this.NameBot = null;
            this.disposed = true;
            this.handler.TrgBotCollision -= this.delegateFunction;
            this.handler = null;
            this.item = null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.item.Id, string.Empty, this.NameBot, false, null);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            this.NameBot = row["trigger_data"].ToString();
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.WIRED_TRIGGER);
            Message.WriteBoolean(false);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(SpriteId);
            Message.WriteInteger(this.item.Id);
            Message.WriteString(this.NameBot);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(14);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Session.SendPacket(Message);
        }
    }
}
