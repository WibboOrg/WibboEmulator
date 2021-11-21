using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Triggers
{
    public class EntersRoom : IWired
    {
        private Item item;
        private WiredHandler handler;
        private bool isOneUser;
        private string userName;
        private readonly RoomEventDelegate delegateFunction;

        public EntersRoom(Item item, WiredHandler handler, RoomUserManager roomUserManager, bool isOneUser, string userName)
        {
            this.item = item;
            this.handler = handler;
            this.isOneUser = isOneUser;
            this.userName = userName;
            this.delegateFunction = new RoomEventDelegate(this.OnUserEnter);
            roomUserManager.OnUserEnter += this.delegateFunction;
        }

        private void OnUserEnter(object sender, EventArgs e)
        {
            RoomUser user = (RoomUser)sender;
            if (user == null)
            {
                return;
            }

            if ((user.IsBot || !this.isOneUser || (string.IsNullOrEmpty(this.userName) || !(user.GetUsername() == this.userName))) && this.isOneUser)
            {
                return;
            }

            if (this.handler != null)
            {
                this.handler.ExecutePile(this.item.Coordinate, user, null);
            }
        }

        public void Dispose()
        {
            this.handler = null;
            this.userName = null;
            if (this.item != null && this.item.GetRoom() != null)
            {
                this.item.GetRoom().GetRoomUserManager().OnUserEnter -= this.delegateFunction;
            }

            this.item = null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.item.Id, string.Empty, this.userName, false, null);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            this.userName = row["trigger_data"].ToString();
            this.isOneUser = !string.IsNullOrEmpty(this.userName);
        }

        public void OnTrigger(Client Session, int SpriteId)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.WIRED_TRIGGER);
            Message.WriteBoolean(false);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(SpriteId);
            Message.WriteInteger(this.item.Id);
            Message.WriteString(this.userName);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(7);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Session.SendPacket(Message);
        }

    }
}
