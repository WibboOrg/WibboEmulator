using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;
using Butterfly.Game.Items.Wired.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Butterfly.Game.Items.Wired.Actions
{
    public class HighScore : WiredActionBase, IWired, IWiredEffect
    {
        public HighScore(Item item, Room room) : base(item, room, -1)
        {
        }

        public override bool OnCycle(RoomUser user, Item item)
        {
            if (user == null || user.IsBot || user.GetClient() == null)
            {
                return false;
            }

            Dictionary<string, int> Scores = this.ItemInstance.Scores;

            List<string> ListUsernameScore = new List<string>() { user.GetUsername() };

            if (Scores.ContainsKey(ListUsernameScore[0]))
            {
                Scores[ListUsernameScore[0]] += 1;
            }
            else
            {
                Scores.Add(ListUsernameScore[0], 1);
            }

            this.RoomInstance.SendPacket(new ObjectUpdateComposer(this.ItemInstance, this.RoomInstance.RoomData.OwnerId));

            return false;
        }

        public override void Dispose()
        {
            base.Dispose();

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                this.SaveToDatabase(dbClient);
            }
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            string triggerItems = "";

            foreach (KeyValuePair<string, int> score in this.ItemInstance.Scores.OrderByDescending(x => x.Value))
            {
                triggerItems += score.Key + ":" + score.Value + ";";
            }

            triggerItems = triggerItems.TrimEnd(';');

            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, triggerItems, false, null, this.Delay);
        }

        public void LoadFromDatabase(DataRow row)
        {
            if (int.TryParse(row["delay"].ToString(), out int delay))
	            this.Delay = delay;
                
            string triggerData = row["trigger_data"].ToString();

            if (triggerData == "")
            {
                return;
            }

            foreach (string data in triggerData.Split(';'))
            {
                string[] userData = data.Split(':');

                int.TryParse(userData[userData.Count() - 1], out int score);

                string username = "";

                for (int i = 0; i < userData.Count() - 1; i++)
                {
                    if (i == 0)
                    {
                        username = userData[i];
                    }
                    else
                    {
                        username += ':' + userData[i];
                    }
                }

                if (!this.ItemInstance.Scores.ContainsKey(username))
                {
                    this.ItemInstance.Scores.Add(username, score);
                }
            }
        }

        public override void OnTrigger(Client Session)
        {
            int.TryParse(this.ItemInstance.ExtraData, out int NumMode);

            if (NumMode != 1)
            {
                NumMode = 1;
            }
            else
            {
                NumMode = 0;
            }

            this.ItemInstance.ExtraData = NumMode.ToString();
            this.ItemInstance.UpdateState(false, true);
        }
    }
}
