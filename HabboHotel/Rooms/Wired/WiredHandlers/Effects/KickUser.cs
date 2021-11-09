using System.Data;
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

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            this.message = row["trigger_data"].ToString();
        }

        public void OnTrigger(GameClient Session, int SpriteId)
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
            Message.WriteInteger(0);
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
