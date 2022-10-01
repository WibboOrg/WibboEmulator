using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Items.Wired.Interfaces;
using System.Data;
using WibboEmulator.Utilities.Events;

namespace WibboEmulator.Games.Items.Wired.Triggers
{
    public class ScoreAchieved : WiredTriggerBase, IWired
    {
        private readonly TeamScoreChangedDelegate scoreChangedDelegate;

        public ScoreAchieved(Item item, Room room) : base(item, room, (int)WiredTriggerType.SCORE_ACHIEVED)
        {
            this.scoreChangedDelegate = new TeamScoreChangedDelegate(this.OnScoreChanged);
            this.RoomInstance.GetGameManager().OnScoreChanged += this.scoreChangedDelegate;

            this.IntParams.Add(0);
        }

        private void OnScoreChanged(object sender, TeamScoreChangedArgs e)
        {
            int scoreLevel = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0);
            if (e.Points <= scoreLevel - 1)
            {
                return;
            }

            this.RoomInstance.GetWiredHandler().ExecutePile(this.ItemInstance.Coordinate, e.user, null);
        }

        public override void Dispose()
        {
            this.RoomInstance.GetWiredHandler().GetRoom().GetGameManager().OnScoreChanged -= this.scoreChangedDelegate;

            base.Dispose();
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            int scoreLevel = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0);
            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, scoreLevel.ToString(), false, null);
        }

        public void LoadFromDatabase(DataRow row)
        {
            this.IntParams.Clear();

            if (int.TryParse(row["trigger_data"].ToString(), out int score))
                this.IntParams.Add(score);
        }
    }
}
