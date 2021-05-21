using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;

namespace Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Effects
{
    public class KickUser : IWired, IWiredCycleable, IWiredEffect
    {
        private WiredHandler handler;
        private readonly int itemID;
        private string message;
        public int Delay { get; set; }
        private bool disposed;
        private readonly Room mRoom;

        public KickUser(string message, WiredHandler handler, int itemID, Room room)
        {
            this.itemID = itemID;
            this.handler = handler;
            this.message = message;
            this.mRoom = room;
            this.Delay = 2;
        }

        public bool OnCycle(RoomUser user, Item item)
        {
            if (user != null && user.GetClient() != null)
            {
                if (user.RoomId == this.mRoom.RoomData.Id)
                {
                    this.mRoom.GetRoomUserManager().RemoveUserFromRoom(user.GetClient(), true, true);
                }
            }
            return false;
        }

        public void Handle(RoomUser User, Item TriggerItem)
        {
            if (User != null && User.GetClient() != null && User.GetClient().GetHabbo() != null)
            {
                if (User.GetClient().GetHabbo().HasFuse("fuse_mod") || this.mRoom.RoomData.OwnerId == User.UserId)
                {
                    if (User.GetClient() != null)
                    {
                        User.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("wired.kick.exception", User.GetClient().Langue));
                    }

                    return;
                }

                User.ApplyEffect(4);
                User.Freeze = true;
                if (!string.IsNullOrEmpty(this.message))
                {
                    User.SendWhisperChat(this.message);
                }

                this.handler.RequestCycle(new WiredCycle(this, User, null, this.Delay));
            }
        }

        public void Dispose()
        {
            this.disposed = true;
            this.handler = null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.itemID, string.Empty, this.message, false, null);
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            dbClient.SetQuery("SELECT trigger_data FROM wired_items WHERE trigger_id = @id ");
            dbClient.AddParameter("id", this.itemID);
            this.message = dbClient.GetString();
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message15 = new ServerPacket(ServerPacketHeader.WIRED_ACTION);
            Message15.WriteBoolean(false);
            Message15.WriteInteger(0);
            Message15.WriteInteger(0);
            Message15.WriteInteger(SpriteId);
            Message15.WriteInteger(this.itemID);
            Message15.WriteString(this.message);
            Message15.WriteInteger(0);
            Message15.WriteInteger(0);
            Message15.WriteInteger(7); //7
            Message15.WriteInteger(0);
            Message15.WriteInteger(0);
            Message15.WriteInteger(0);
            Session.SendPacket(Message15);
        }

        public void DeleteFromDatabase(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM wired_items WHERE trigger_id = '" + this.itemID + "'");
        }

        public bool Disposed()
        {
            return this.disposed;
        }
    }
}
