using Butterfly.Database.Interfaces;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Actions
{
    public class BotTalk : WiredActionBase, IWired, IWiredEffect
    {
        public BotTalk(Item item, Room room) : base(item, room, (int)WiredActionType.BOT_TALK)
        {
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (!this.StringParam.Contains("\t"))
                return;

            string[] messageAndName = this.StringParam.Split('\t');
            string name = (messageAndName.Length == 2) ? messageAndName[0] : "";
            string message = (messageAndName.Length == 2) ? messageAndName[1] : "";

            if (name == "" || message == "")
            {
                return;
            }

            RoomUser Bot = this.RoomInstance.GetRoomUserManager().GetBotOrPetByName(name);
            if (Bot == null)
            {
                return;
            }

            string TextMessage = message;
            if (user != null)
            {
                TextMessage = TextMessage.Replace("#username#", user.GetUsername());
                TextMessage = TextMessage.Replace("#point#", user.WiredPoints.ToString());
                TextMessage = TextMessage.Replace("#roomname#", this.RoomInstance.RoomData.Name.ToString());
                TextMessage = TextMessage.Replace("#vote_yes#", this.RoomInstance.VotedYesCount.ToString());
                TextMessage = TextMessage.Replace("#vote_no#", this.RoomInstance.VotedNoCount.ToString());

                if (user.Roleplayer != null)
                {
                    TextMessage = TextMessage.Replace("#money#", user.Roleplayer.Money.ToString());
                    TextMessage = TextMessage.Replace("#money1#", user.Roleplayer.Money1.ToString());
                    TextMessage = TextMessage.Replace("#money2#", user.Roleplayer.Money2.ToString());
                    TextMessage = TextMessage.Replace("#money3#", user.Roleplayer.Money3.ToString());
                    TextMessage = TextMessage.Replace("#money4#", user.Roleplayer.Money4.ToString());
                }
            }

            bool isShout = (((this.IntParams.Count > 0) ? this.IntParams[0] : 0)) == 1;

            Bot.OnChat(TextMessage, (Bot.IsPet) ? 0 : 2, isShout);

        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            bool isShout = (((this.IntParams.Count > 0) ? this.IntParams[0] : 0)) == 1;

            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, this.StringParam, isShout, null);
        }

        public void LoadFromDatabase(DataRow row)
        {
            if (int.TryParse(row["all_user_triggerable"].ToString(), out int isShout))
                this.IntParams.Add(isShout);

            string Data = row["trigger_data"].ToString();

            if (!Data.Contains("\t"))
            {
                return;
            }

            this.StringParam = Data;
        }
    }
}
