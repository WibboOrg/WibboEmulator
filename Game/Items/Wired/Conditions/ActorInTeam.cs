using Butterfly.Database.Interfaces;
using Butterfly.Game.Rooms;
using Butterfly.Game.Rooms.Games;
using Butterfly.Game.Items.Wired.Interfaces;
using System.Data;

namespace Butterfly.Game.Items.Wired.Conditions
{
    public class ActorInTeam : WiredConditionBase, IWiredCondition, IWired
    {
        public ActorInTeam(Item item, Room room) : base(item, room, (int)WiredConditionType.ACTOR_IS_IN_TEAM)
        {
        }

        public override void LoadItems(bool inDatabase = false)
        {
            base.LoadItems(inDatabase);

            if (inDatabase)
                return;

            this.IntParams.Add((int)TeamType.red);
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

            if (user.Team != (TeamType)teamId)
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
            if (int.TryParse(row["trigger_data"].ToString(), out int teamId))
                this.IntParams.Add(teamId);
        }
    }
}
