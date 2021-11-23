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

        public void Handle(RoomUser user, Item TriggerItem)
        {
            int scoreToGive = (this.IntParams.Count > 0) ? this.IntParams[0] : 0;
            int maxCountPerGame = (this.IntParams.Count > 1) ? this.IntParams[1] : 0;

            if (user == null || user.Team == Team.none || maxCountPerGame <= this.currentGameCount)
            {
                return;
            }

            this.currentGameCount++;
            this.RoomInstance.GetGameManager().AddPointToTeam(user.Team, scoreToGive, user);
        }

        public override void Dispose()
        {
            this.RoomInstance.GetGameManager().OnGameStart -= this.delegateFunction;
            this.delegateFunction = null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            int scoreToGive = (this.IntParams.Count > 0) ? this.IntParams[0] : 0;
            int maxCountPerGame = (this.IntParams.Count > 1) ? this.IntParams[1] : 0;

            WiredUtillity.SaveTriggerItem(dbClient, this.ItemInstance.Id, scoreToGive.ToString(), maxCountPerGame.ToString(), false, null);
        }

        public void LoadFromDatabase(DataRow row)
        {
            if (int.TryParse(row["trigger_data"].ToString(), out int maxCount))
                this.IntParams.Add(maxCount);

            if (int.TryParse(row["trigger_data_2"].ToString(), out int score))
                this.IntParams.Add(score);
        }
    }
}
