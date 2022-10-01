using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Rooms;
using WibboEmulator.Game.Rooms.Games;
using WibboEmulator.Game.Items.Wired.Interfaces;
using System.Data;

namespace WibboEmulator.Game.Items.Wired.Conditions
{
    public class ActorNotInTeam : WiredConditionBase, IWiredCondition, IWired
    {
        public ActorNotInTeam(Item item, Room room) : base(item, room, (int)WiredConditionType.NOT_ACTOR_IN_TEAM)
        {
            this.IntParams.Add((int)TeamType.RED);
        }

        public bool AllowsExecution(RoomUser user, Item TriggerItem)
        {
            if (user == null)
            {
                return false;
            }

            int teamId = (this.IntParams.Count > 0) ? this.IntParams[0] : 1;
            if (teamId < 1 || teamId > 4)
            {
                teamId = 1;
            }

            if (user.Team == (TeamType)teamId)
            {
                return false;
            }

            return true;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            int teamId = (this.IntParams.Count > 0) ? this.IntParams[0] : 1;

            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, teamId.ToString(), false, null);
        }

        public void LoadFromDatabase(DataRow row)
        {
            this.IntParams.Clear();

            if (int.TryParse(row["trigger_data"].ToString(), out int teamId))
                this.IntParams.Add(teamId);
        }
    }
}
