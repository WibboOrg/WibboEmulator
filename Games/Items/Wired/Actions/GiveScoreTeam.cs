using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games;

namespace WibboEmulator.Games.Items.Wired.Actions
{
    public class GiveScoreTeam : WiredActionBase, IWiredEffect, IWired
    {
        private RoomEventDelegate delegateFunction;
        private int currentGameCount;

        public GiveScoreTeam(Item item, Room room) : base(item, room, (int)WiredActionType.GIVE_SCORE_TO_PREDEFINED_TEAM)
        {
            this.currentGameCount = 0;
            this.delegateFunction = new RoomEventDelegate(this.OnGameStart);
            this.RoomInstance.GetGameManager().OnGameStart += this.delegateFunction;

            this.IntParams.Add(1);
            this.IntParams.Add(1);
            this.IntParams.Add((int)TeamType.RED);
        }

        private void OnGameStart(object sender, EventArgs e) => this.currentGameCount = 0;

        public override bool OnCycle(RoomUser user, Item item)
        {
            int scoreToGive = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0);
            int maxCountPerGame = ((this.IntParams.Count > 1) ? this.IntParams[1] : 0);
            TeamType team = (TeamType)((this.IntParams.Count > 2) ? this.IntParams[2] : 1);

            if (maxCountPerGame <= this.currentGameCount)
            {
                return false;
            }

            this.currentGameCount++;
            this.RoomInstance.GetGameManager().AddPointToTeam(team, scoreToGive, user);

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
            int scoreToGive = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0);
            int maxCountPerGame = ((this.IntParams.Count > 1) ? this.IntParams[1] : 0);
            int team = ((this.IntParams.Count > 2) ? this.IntParams[2] : 0);

            WiredUtillity.SaveTriggerItem(dbClient, this.Id, team.ToString(), maxCountPerGame.ToString() + ":" + scoreToGive.ToString(), false, null, this.Delay);
        }

        public void LoadFromDatabase(DataRow row)
        {
            this.IntParams.Clear();

            if (int.TryParse(row["delay"].ToString(), out int delay))
                this.Delay = delay;

            string triggerData = row["trigger_data"].ToString();
            string triggerData2 = row["trigger_data_2"].ToString();

            if (triggerData == null || !triggerData.Contains(':'))
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
