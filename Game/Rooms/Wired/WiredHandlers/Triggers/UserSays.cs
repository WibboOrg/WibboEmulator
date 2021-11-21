using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Triggers
{
    public class UserSays : IWired
    {
        private Item item;
        private WiredHandler handler;
        private bool isOwnerOnly;
        private string triggerMessage;
        private readonly RoomUserSaysDelegate delegateFunction;

        public UserSays(Item item, WiredHandler handler, bool isOwnerOnly, string triggerMessage, Room room)
        {
            this.item = item;
            this.handler = handler;
            this.isOwnerOnly = isOwnerOnly;
            this.triggerMessage = triggerMessage;
            this.delegateFunction = new RoomUserSaysDelegate(this.OnUserSays);
            room.OnUserSays += this.delegateFunction;
        }

        private void OnUserSays(object sender, UserSaysArgs e, ref bool messageHandled)
        {
            RoomUser user = e.User;
            string message = e.Message;

            if (user != null && (!this.isOwnerOnly && this.canBeTriggered(message) && !string.IsNullOrEmpty(message)) || (this.isOwnerOnly && user.IsOwner() && this.canBeTriggered(message) && !string.IsNullOrEmpty(message)))
            {
                this.handler.ExecutePile(this.item.Coordinate, user, null);
                messageHandled = true;
            }
        }

        private bool canBeTriggered(string message)
        {
            if (string.IsNullOrEmpty(this.triggerMessage))
            {
                return false;
            }

            return message.ToLower() == this.triggerMessage.ToLower();
        }

        public void Dispose()
        {
            this.handler.GetRoom().OnUserSays -= this.delegateFunction;
            this.item = null;
            this.handler = null;
            this.triggerMessage = null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.item.Id, string.Empty, this.triggerMessage, this.isOwnerOnly, null);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            this.triggerMessage = row["trigger_data"].ToString();
            this.isOwnerOnly = row["all_user_triggerable"].ToString() == "1";
        }

        public void OnTrigger(Client Session, int SpriteId)
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
