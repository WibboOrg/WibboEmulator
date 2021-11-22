using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Games;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Conditions
{
    public class ActorInTeam : WiredConditionBase, IWiredCondition, IWired
    {
        public ActorInTeam(Item item, List<int> intParams) : base()
        {
            this.Id = item.Id;
            this.Type = (int)WiredConditionType.ACTOR_IS_IN_TEAM;
            this.StuffTypeId = item.GetBaseItem().SpriteId;
            this.IntParams = intParams;
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

            if (user.Team != (Team)teamId)
            {
                return false;
            }

            return true;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            int teamId = (this.IntParams.Count > 0) ? this.IntParams[0] : 1;
            if (teamId < 1 || teamId > 4)
            {
                teamId = 1;
            }

            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, teamId.ToString(), false, null);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            if (int.TryParse(row["trigger_data"].ToString(), out int teamId))
            {
                this.IntParams.Add(teamId);
            }
        }

        public void OnTrigger(Client Session, int spriteId)
        {
            this.OnTrigger(Session);
        }
    }
}
