using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Games;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Actions
{
    public class GiveScoreTeam : WiredActionBase, IWiredEffect, IWired
    {
        private RoomEventDelegate delegateFunction;
        private int currentGameCount;

        public GiveScoreTeam(Item item, Room room) : base(item, room, (int)WiredActionType.GIVE_SCORE_TO_PREDEFINED_TEAM)
        {
            this.currentGameCount = 0;
            this.delegateFunction = new RoomEventDelegate(this.gameManager_OnGameStart);
            this.RoomInstance.GetGameManager().OnGameStart += this.delegateFunction;
        }

        private void gameManager_OnGameStart(object sender, EventArgs e)
        {
            this.currentGameCount = 0;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            int scoreToGive = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0);
            int maxCountPerGame = ((this.IntParams.Count > 1) ? this.IntParams[1] : 0);
            Team team = (Team)((this.IntParams.Count > 2) ? this.IntParams[2] : 0);

            if (maxCountPerGame <= this.currentGameCount)
            {
                return;
            }

            this.currentGameCount++;
            this.RoomInstance.GetGameManager().AddPointToTeam(team, scoreToGive, user);
        }

        public override void Dispose()
        {
            this.RoomInstance.GetGameManager().OnGameStart -= this.delegateFunction;
            this.delegateFunction = null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            int scoreToGive = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0);
            int maxCountPerGame = ((this.IntParams.Count > 1) ? this.IntParams[1] : 0);
            int team = ((this.IntParams.Count > 2) ? this.IntParams[2] : 0);

            WiredUtillity.SaveTriggerItem(dbClient, this.Id, team.ToString(), maxCountPerGame.ToString() + ":" + scoreToGive.ToString(), false, null);
        }

        public void LoadFromDatabase(DataRow row)
        {
            string triggerData = row["trigger_data"].ToString();
            string triggerData2 = row["trigger_data_2"].ToString();

            if (!triggerData.Contains(":"))
                return;

            string[] dataSplit = triggerData.Split(':');

            if (int.TryParse(dataSplit[1], out int score))
                this.IntParams.Add(score);

                if (int.TryParse(dataSplit[0], out int maxCount))
                this.IntParams.Add(maxCount);

            if (int.TryParse(triggerData2, out int team))
                this.IntParams.Add(team);

        }
    }
}
