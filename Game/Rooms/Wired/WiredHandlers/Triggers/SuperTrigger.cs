using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.GameClients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Triggers
{
    public class SuperTrigger : IWired
    {
        private Item item;
        private WiredHandler handler;
        private string triggerMessage;
        private readonly TriggerUserDelegate delegateFunction;

        public SuperTrigger(Item item, WiredHandler handler, string triggerMessage, Room room)
        {
            switch (triggerMessage)
            {
                case "test":
                    this.triggerMessage = triggerMessage;
                    break;
            }

            this.item = item;
            this.handler = handler;
            this.delegateFunction = new TriggerUserDelegate(this.roomUserManager_SuperTrigger);
            room.TriggerUser += this.delegateFunction;
        }

        private void roomUserManager_SuperTrigger(RoomUser user, string ActionType)
        {
            if (ActionType == this.triggerMessage)
            {
                this.handler.ExecutePile(this.item.Coordinate, user, null);
            }
        }

        public void Dispose()
        {
            this.handler.GetRoom().TriggerUser -= this.delegateFunction;
            this.item = null;
            this.handler = null;
            this.triggerMessage = null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.item.Id, string.Empty, this.triggerMessage, false, null);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            this.triggerMessage = row["trigger_data"].ToString();
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.WIRED_TRIGGER);
            Message.WriteBoolean(false);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(SpriteId);
            Message.WriteInteger(this.item.Id);
            Message.WriteString(this.triggerMessage);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Session.SendPacket(Message);
        }
    }
}
