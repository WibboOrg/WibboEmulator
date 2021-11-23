using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Games;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Conditions
{
    public class ActorNotInTeam : WiredConditionBase, IWiredCondition, IWired
    {
        public ActorNotInTeam(Item item, Room room) : base(item, room, (int)WiredConditionType.NOT_ACTOR_IN_TEAM)
        {
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

            if (user.Team == (Team)teamId)
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
