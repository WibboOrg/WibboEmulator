using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Effects
{
    public class BotClothes : IWired, IWiredEffect
    {
        private readonly WiredHandler handler;
        private readonly int itemID;
        private string NameBot;
        private string Look;

        public BotClothes(string namebot, string look, WiredHandler handler, int itemID)
        {
            this.itemID = itemID;
            this.handler = handler;
            this.NameBot = namebot;
            this.Look = look;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (this.NameBot == "" || this.Look == "")
            {
                return;
            }

            Room room = this.handler.GetRoom();
            RoomUser Bot = room.GetRoomUserManager().GetBotOrPetByName(this.NameBot);
            if (Bot == null)
            {
                return;
            }

            Bot.BotData.Look = this.Look;

            room.SendPacket(new UserChangeComposer(Bot));
        }

        public void Dispose()
        {
            this.NameBot = null;
            this.Look = null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.itemID, string.Empty, this.NameBot + '\t' + this.Look, false, null);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            string Data = row["trigger_data"].ToString();

            if (string.IsNullOrWhiteSpace(Data) || !Data.Contains("\t"))
            {
                return;
            }

            string[] SplitData = Data.Split('\t');

            this.NameBot = SplitData[0].ToString();
            this.Look = SplitData[1].ToString();
        }

        public void OnTrigger(Client Session, int SpriteId)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.WIRED_ACTION);
            Message.WriteBoolean(false);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(SpriteId);
            Message.WriteInteger(this.itemID);
            Message.WriteString(this.NameBot + '\t' + this.Look);
            Message.WriteInteger(0);

            Message.WriteInteger(0);
            Message.WriteInteger(26); //7
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Session.SendPacket(Message);
        }
    }
}
