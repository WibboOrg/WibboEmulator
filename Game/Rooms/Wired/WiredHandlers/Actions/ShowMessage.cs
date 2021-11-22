using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Actions
{
    public class ShowMessage : IWired, IWiredEffect, IWiredCycleable
    {
        private readonly WiredHandler handler;
        private readonly int itemID;
        private string message;
        public int DelayCycle { get; set; }
        private bool disposed;

        public ShowMessage(string message, WiredHandler handler, int itemID, int mdelay)
        {
            this.itemID = itemID;
            this.handler = handler;
            this.message = message;

            this.DelayCycle = mdelay;
            this.disposed = false;
        }

        private void HandleEffect(RoomUser user, Item TriggerItem)
        {
            if (this.message == "")
            {
                return;
            }

            if (user != null && !user.IsBot && user.GetClient() != null)
            {
                string TextMessage = this.message;
                TextMessage = TextMessage.Replace("#username#", user.GetUsername());
                TextMessage = TextMessage.Replace("#point#", user.WiredPoints.ToString());
                TextMessage = TextMessage.Replace("#roomname#", this.handler.GetRoom().RoomData.Name.ToString());
                TextMessage = TextMessage.Replace("#vote_yes#", this.handler.GetRoom().VotedYesCount.ToString());
                TextMessage = TextMessage.Replace("#vote_no#", this.handler.GetRoom().VotedNoCount.ToString());

                if (user.Roleplayer != null)
                {
                    TextMessage = TextMessage.Replace("#money#", user.Roleplayer.Money.ToString());
                }

                user.SendWhisperChat(TextMessage);
            }
        }

        public bool OnCycle(RoomUser user, Item item)
        {
            this.HandleEffect(user, item);
            return false;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (user == null || user.IsBot || user.GetClient() == null)
            {
                return;
            }

            if (this.message == "")
            {
                return;
            }

            if (this.DelayCycle > 0)
            {
                this.handler.RequestCycle(new WiredCycle(this, user, TriggerItem, this.DelayCycle));
            }
            else
            {
                this.HandleEffect(user, TriggerItem);
            }
        }

        public void Dispose()
        {
            this.disposed = true;
            this.message = null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.itemID, this.DelayCycle.ToString(), this.message, false, null);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            this.message = row["trigger_data"].ToString();

            if (int.TryParse(row["trigger_data_2"].ToString(), out int delay))
                this.DelayCycle = delay;
        }

        public void OnTrigger(Client Session, int SpriteId)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.WIRED_ACTION);
            Message.WriteBoolean(false);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(SpriteId);
            Message.WriteInteger(this.itemID);
            Message.WriteString(this.message);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(7); //7
            Message.WriteInteger(this.DelayCycle);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Session.SendPacket(Message);
        }

        public bool Disposed()
        {
            return this.disposed;
        }
    }
}
