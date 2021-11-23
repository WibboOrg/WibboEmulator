using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Actions
{
    public class ShowMessage : WiredActionBase, IWired, IWiredEffect, IWiredCycleable
    {
        public ShowMessage(Item item, Room room) : base(item, room, (int)WiredActionType.CHAT)
        {
        }

        private void HandleEffect(RoomUser user, Item TriggerItem)
        {
            if (this.StringParam == "")
            {
                return;
            }

            if (user != null && !user.IsBot && user.GetClient() != null)
            {
                string TextMessage = this.StringParam;
                TextMessage = TextMessage.Replace("#username#", user.GetUsername());
                TextMessage = TextMessage.Replace("#point#", user.WiredPoints.ToString());
                TextMessage = TextMessage.Replace("#roomname#", this.RoomInstance.GetWiredHandler().GetRoom().RoomData.Name.ToString());
                TextMessage = TextMessage.Replace("#vote_yes#", this.RoomInstance.GetWiredHandler().GetRoom().VotedYesCount.ToString());
                TextMessage = TextMessage.Replace("#vote_no#", this.RoomInstance.GetWiredHandler().GetRoom().VotedNoCount.ToString());

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

            if (this.StringParam == "")
            {
                return;
            }

            if (this.DelayCycle > 0)
            {
                this.RoomInstance.GetWiredHandler().RequestCycle(new WiredCycle(this, user, TriggerItem, this.DelayCycle));
            }
            else
            {
                this.HandleEffect(user, TriggerItem);
            }
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.Id, this.DelayCycle.ToString(), this.StringParam, false, null);
        }

        public void LoadFromDatabase(DataRow row)
        {
            this.StringParam = row["trigger_data"].ToString();

            if (int.TryParse(row["trigger_data_2"].ToString(), out int delay))
                this.Delay = delay;
        }
    }
}
