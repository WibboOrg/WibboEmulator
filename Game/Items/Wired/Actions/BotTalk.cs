using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Rooms;
using WibboEmulator.Game.Items.Wired.Interfaces;
using System.Data;

namespace WibboEmulator.Game.Items.Wired.Actions
{
    public class BotTalk : WiredActionBase, IWired, IWiredEffect
    {
        public BotTalk(Item item, Room room) : base(item, room, (int)WiredActionType.BOT_TALK)
        {
            this.IntParams.Add(0);
        }

        public override bool OnCycle(RoomUser user, Item item)
        {
            if (!this.StringParam.Contains("\t"))
                return false;

            string[] messageAndName = this.StringParam.Split('\t');
            string name = (messageAndName.Length == 2) ? messageAndName[0] : "";
            string message = (messageAndName.Length == 2) ? messageAndName[1] : "";

            if (name == "" || message == "")
            {
                return false;
            }

            RoomUser bot = this.RoomInstance.GetRoomUserManager().GetBotOrPetByName(name);
            if (bot == null)
            {
                return false;
            }

            string textMessage = message;
            if (user != null && user.GetClient() != null)
            {
                textMessage = textMessage.Replace("#username#", user.GetUsername());
                textMessage = textMessage.Replace("#point#", user.WiredPoints.ToString());
                textMessage = textMessage.Replace("#roomname#", this.RoomInstance.RoomData.Name.ToString());
                textMessage = textMessage.Replace("#vote_yes#", this.RoomInstance.VotedYesCount.ToString());
                textMessage = textMessage.Replace("#vote_no#", this.RoomInstance.VotedNoCount.ToString());
                textMessage = textMessage.Replace("#wpcount#", user.GetClient().GetUser() != null ? user.GetClient().GetUser().WibboPoints.ToString() : "0");

                if (user.Roleplayer != null)
                {
                    textMessage = textMessage.Replace("#money#", user.Roleplayer.Money.ToString());
                }
            }

            bool isShout = (((this.IntParams.Count > 0) ? this.IntParams[0] : 0)) == 1;

            bot.OnChat(textMessage, (bot.IsPet) ? 0 : 2, isShout);

            return false;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            bool isShout = (((this.IntParams.Count > 0) ? this.IntParams[0] : 0)) == 1;

            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, this.StringParam, isShout, null, this.Delay);
        }

        public void LoadFromDatabase(DataRow row)
        {
            this.IntParams.Clear();

            if (int.TryParse(row["delay"].ToString(), out int delay))
	            this.Delay = delay;
                
            if (bool.TryParse(row["all_user_triggerable"].ToString(), out bool isShout))
                this.IntParams.Add(isShout ? 1 : 0);

            string Data = row["trigger_data"].ToString();

            if (!Data.Contains("\t"))
            {
                return;
            }

            this.StringParam = Data;
        }
    }
}
