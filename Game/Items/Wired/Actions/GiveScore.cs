using Butterfly.Database.Interfaces;
using Butterfly.Game.Rooms;
using Butterfly.Game.Rooms.Games;
using Butterfly.Game.Items.Wired.Interfaces;
using System;
using System.Data;

namespace Butterfly.Game.Items.Wired.Actions
{
    public class GiveScore : WiredActionBase, IWiredEffect, IWired
    {
        private int currentGameCount;
        private RoomEventDelegate delegateFunction;

        public GiveScore(Item item, Room room) : base(item, room, (int)WiredActionType.GIVE_SCORE)
        {
            this.currentGameCount = 0;
            this.delegateFunction = new RoomEventDelegate(this.gameManager_OnGameStart);
            this.RoomInstance.GetGameManager().OnGameStart += this.delegateFunction;
        }

        private void gameManager_OnGameStart(object sender, EventArgs e)
        {
            this.currentGameCount = 0;
        }

        public override bool OnCycle(RoomUser user, Item item)
        {
            int scoreToGive = (this.IntParams.Count > 0) ? this.IntParams[0] : 0;
            int maxCountPerGame = (this.IntParams.Count > 1) ? this.IntParams[1] : 0;

            if (user == null || user.Team == TeamType.none || maxCountPerGame <= this.currentGameCount)
            {
                return false;
            }

            this.currentGameCount++;
            this.RoomInstance.GetGameManager().AddPointToTeam(user.Team, scoreToGive, user);

            return false;
        }

        public override void Dispose()
        {
            base.Dispose();

            this.RoomInstance.GetGameManager().OnGameStart -= this.delegateFunction;
            this.delegateFunction = null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            int scoreToGive = (this.IntParams.Count > 0) ? this.IntParams[0] : 0;
            int maxCountPerGame = (this.IntParams.Count > 1) ? this.IntParams[1] : 0;

            WiredUtillity.SaveTriggerItem(dbClient, this.ItemInstance.Id, scoreToGive.ToString(), maxCountPerGame.ToString(), false, null, this.Delay);
        }

        public void LoadFromDatabase(DataRow row)
        {
            if (int.TryParse(row["delay"].ToString(), out int delay))
	            this.Delay = delay;
                
            if (int.TryParse(row["trigger_data"].ToString(), out int maxCount))
                this.IntParams.Add(maxCount);

            if (int.TryParse(row["trigger_data_2"].ToString(), out int score))
                this.IntParams.Add(score);
        }
    }
}
