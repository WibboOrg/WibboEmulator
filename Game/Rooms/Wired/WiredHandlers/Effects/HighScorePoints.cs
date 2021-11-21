using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Effects
{
    public class HighScorePoints : IWired, IWiredEffect
    {
        private readonly Item item;

        public HighScorePoints(Item item)
        {
            this.item = item;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (user == null || user.IsBot || user.GetClient() == null)
            {
                return;
            }

            Dictionary<string, int> Scores = this.item.Scores;

            List<string> ListUsernameScore = new List<string>() { user.GetUsername() };

            if (Scores.ContainsKey(ListUsernameScore[0]))
            {
                if (user.WiredPoints > Scores[ListUsernameScore[0]])
                {
                    Scores[ListUsernameScore[0]] = user.WiredPoints;
                }
            }
            else
            {
                Scores.Add(ListUsernameScore[0], user.WiredPoints);
            }

            Room room = this.item.GetRoom();
            if (room == null)
            {
                return;
            }

            room.SendPacket(new ObjectUpdateComposer(this.item, room.RoomData.OwnerId));

        }

        public void Dispose()
        {
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                this.SaveToDatabase(dbClient);
            }
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            string triggerdata = "";

            int i = 0;
            foreach (KeyValuePair<string, int> score in this.item.Scores.OrderByDescending(x => x.Value).Take(20))
            {
                if (i != 0)
                {
                    triggerdata += ";";
                }

                triggerdata += score.Key + ":" + score.Value;

                i++;
            }

            WiredUtillity.SaveTriggerItem(dbClient, this.item.Id, string.Empty, triggerdata, false, null);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
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

                if (!this.item.Scores.ContainsKey(username))
                {
                    this.item.Scores.Add(username, ScoreNum);
                }
            }
        }

        public void OnTrigger(Client Session, int SpriteId)
        {
            int.TryParse(this.item.ExtraData, out int NumMode);

            if (NumMode != 1)
            {
                NumMode = 1;
            }
            else
            {
                NumMode = 0;
            }

            this.item.ExtraData = NumMode.ToString();
            this.item.UpdateState(false, true);
        }
    }
}
