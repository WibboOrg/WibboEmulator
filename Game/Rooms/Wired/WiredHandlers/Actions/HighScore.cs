using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Actions
{
    public class HighScore : WiredActionBase, IWired, IWiredEffect
    {
        public HighScore(Item item, Room room) : base(item, room, -1)
        {
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (user == null || user.IsBot || user.GetClient() == null)
            {
                return;
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
        }

        public override void Dispose()
        {
            this.IsDisposed = true;

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                this.SaveToDatabase(dbClient);
            }
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            string triggerdata = "";

            int i = 0;
            foreach (KeyValuePair<string, int> score in this.ItemInstance.Scores.OrderByDescending(x => x.Value).Take(20))
            {
                if (i != 0)
                {
                    triggerdata += ";";
                }

                triggerdata += score.Key + ":" + score.Value;

                i++;
            }

            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, triggerdata, false, null);
        }

        public void LoadFromDatabase(DataRow row)
        {
            string triggerData = row["trigger_data"].ToString();

            if (triggerData == "")
            {
                return;
            }

            foreach (string score in triggerData.Split(';'))
            {
                string[] score2 = score.Split(':');
                int.TryParse(score2[score2.Count() - 1], out int ScoreNum);
                string username = "";
                for (int i = 0; i < score2.Count() - 1; i++)
                {
                    if (i == 0)
                    {
                        username = score2[i];
                    }
                    else
                    {
                        username += ':' + score2[i];
                    }
                }

                if (!this.ItemInstance.Scores.ContainsKey(username))
                {
                    this.ItemInstance.Scores.Add(username, ScoreNum);
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
