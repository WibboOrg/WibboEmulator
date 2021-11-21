using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Effects
{
    public class BotFollowAvatar : IWired, IWiredEffect
    {
        private readonly WiredHandler handler;
        private readonly int itemID;
        private string NameBot;
        private bool IsFollow;

        public BotFollowAvatar(string namebot, bool isfollow, WiredHandler handler, int itemID)
        {
            this.itemID = itemID;
            this.handler = handler;
            this.NameBot = namebot;
            this.IsFollow = isfollow;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (string.IsNullOrEmpty(this.NameBot))
            {
                return;
            }

            Room room = this.handler.GetRoom();
            RoomUser Bot = room.GetRoomUserManager().GetBotOrPetByName(this.NameBot);
            if (Bot == null)
            {
                return;
            }

            if (user != null && !user.IsBot && user.GetClient() != null)
            {
                if (this.IsFollow)
                {
                    if (Bot.BotData.FollowUser != user.VirtualId)
                    {
                        Bot.BotData.FollowUser = user.VirtualId;
                    }
                }
                else
                {
                    Bot.BotData.FollowUser = 0;
                }
            }
        }

        public void Dispose()
        {
            this.NameBot = null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.itemID, string.Empty, this.NameBot, this.IsFollow, null);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            this.IsFollow = (row["all_user_triggerable"].ToString() == "1");

            this.NameBot = row["trigger_data"].ToString();
        }

        public void OnTrigger(Client Session, int SpriteId)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.WIRED_ACTION);
            Message.WriteBoolean(false);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(SpriteId);
            Message.WriteInteger(this.itemID);
            Message.WriteString(this.NameBot);
            Message.WriteInteger(1);
            Message.WriteInteger(this.IsFollow ? 1 : 0);

            Message.WriteInteger(0);
            Message.WriteInteger(25); //7
            Message.WriteInteger(0);

            Message.WriteInteger(0);
            Session.SendPacket(Message);
        }
    }
}
