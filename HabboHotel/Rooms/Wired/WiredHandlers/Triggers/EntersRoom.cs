using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Data;

namespace Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Triggers
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

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message2 = new ServerPacket(ServerPacketHeader.WIRED_TRIGGER);
            Message2.WriteBoolean(false);
            Message2.WriteInteger(0);
            Message2.WriteInteger(0);
            Message2.WriteInteger(SpriteId);
            Message2.WriteInteger(this.item.Id);
            Message2.WriteString(this.userName);
            Message2.WriteInteger(0);
            Message2.WriteInteger(0);
            Message2.WriteInteger(7);
            Message2.WriteInteger(0);
            Message2.WriteInteger(0);
            Message2.WriteInteger(0);
            Session.SendPacket(Message2);
        }

    }
}
