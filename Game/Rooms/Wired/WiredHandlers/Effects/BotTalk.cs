using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Effects
{
    public class BotTalk : IWired, IWiredEffect
    {
        private readonly WiredHandler handler;
        private readonly int itemID;
        private string name;
        private string message;
        private bool IsShout;

        public BotTalk(string stringParam, bool isShout, WiredHandler handler, int itemID)
        {
            this.itemID = itemID;
            this.handler = handler;

            string[] messageAndName = stringParam.Split('\t');
            this.message = (messageAndName.Length == 2) ? messageAndName[0] : "";
            this.name = (messageAndName.Length == 2) ? messageAndName[1] : "";
            this.IsShout = isShout;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (this.name == "" || this.message == "")
            {
                return;
            }

            Room room = this.handler.GetRoom();
            RoomUser Bot = room.GetRoomUserManager().GetBotOrPetByName(this.name);
            if (Bot == null)
            {
                return;
            }

            string TextMessage = this.message;
            if (user != null)
            {
                TextMessage = TextMessage.Replace("#username#", user.GetUsername());
                TextMessage = TextMessage.Replace("#point#", user.WiredPoints.ToString());
                TextMessage = TextMessage.Replace("#roomname#", this.handler.GetRoom().RoomData.Name.ToString());
                TextMessage = TextMessage.Replace("#vote_yes#", this.handler.GetRoom().VotedYesCount.ToString());
                TextMessage = TextMessage.Replace("#vote_no#", this.handler.GetRoom().VotedNoCount.ToString());

                if (user.Roleplayer != null)
                {
                    TextMessage = TextMessage.Replace("#money#", user.Roleplayer.Money.ToString());
                    TextMessage = TextMessage.Replace("#money1#", user.Roleplayer.Money1.ToString());
                    TextMessage = TextMessage.Replace("#money2#", user.Roleplayer.Money2.ToString());
                    TextMessage = TextMessage.Replace("#money3#", user.Roleplayer.Money3.ToString());
                    TextMessage = TextMessage.Replace("#money4#", user.Roleplayer.Money4.ToString());
                }
            }

            Bot.OnChat(TextMessage, (Bot.IsPet) ? 0 : 2, this.IsShout);

        }

        public void Dispose()
        {
            this.message = null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.itemID, string.Empty, this.name + '\t' + this.message, this.IsShout, null);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            this.IsShout = (row["all_user_triggerable"].ToString() == "1");

            string Data = row["trigger_data"].ToString();

            if (string.IsNullOrWhiteSpace(Data) || !Data.Contains("\t"))
            {
                return;
            }

            string[] SplitData = Data.Split('\t');

            this.name = SplitData[0].ToString();
            this.message = SplitData[1].ToString();
        }

        public void OnTrigger(Client Session, int SpriteId)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.WIRED_ACTION);
            Message.WriteBoolean(false);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(SpriteId);
            Message.WriteInteger(this.itemID);
            Message.WriteString(this.name + '\t' + this.message);
            Message.WriteInteger(1);
            Message.WriteInteger(this.IsShout ? 1 : 0);
            Message.WriteInteger(0);
            Message.WriteInteger(23); //7
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Session.SendPacket(Message);
        }
    }
}
