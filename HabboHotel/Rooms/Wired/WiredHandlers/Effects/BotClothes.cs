using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;

namespace Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Effects
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

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            dbClient.SetQuery("SELECT trigger_data FROM wired_items WHERE trigger_id = @id ");
            dbClient.AddParameter("id", this.itemID);

            string Data = dbClient.GetString();

            if (string.IsNullOrWhiteSpace(Data) || !Data.Contains("\t"))
            {
                return;
            }

            string[] SplitData = Data.Split('\t');

            this.NameBot = SplitData[0].ToString();
            this.Look = SplitData[1].ToString();
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message15 = new ServerPacket(ServerPacketHeader.WIRED_ACTION);
            Message15.WriteBoolean(false);
            Message15.WriteInteger(0);
            Message15.WriteInteger(0);
            Message15.WriteInteger(SpriteId);
            Message15.WriteInteger(this.itemID);
            Message15.WriteString(this.NameBot + '\t' + this.Look);
            Message15.WriteInteger(0);

            Message15.WriteInteger(0);
            Message15.WriteInteger(26); //7
            Message15.WriteInteger(0);
            Message15.WriteInteger(0);
            Session.SendPacket(Message15);
        }

        public void DeleteFromDatabase(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM wired_items WHERE trigger_id = '" + this.itemID + "'");
        }
    }
}
