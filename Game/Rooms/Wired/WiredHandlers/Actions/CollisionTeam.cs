using Butterfly.Database.Interfaces;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Games;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Actions
{
    public class CollisionTeam : WiredActionBase, IWiredEffect, IWired
    {
        public CollisionTeam(Item item, Room room) : base(item, room, (int)WiredActionType.JOIN_TEAM)
        {
        }

        public override bool OnCycle(RoomUser user, Item item)
        {
            this.HandleItems();

            return false;
        }

        private void HandleItems()
        {
            TeamManager managerForBanzai = this.RoomInstance.GetTeamManager();

            List<RoomUser> ListTeam = new List<RoomUser>();

            Team team = (Team)((this.IntParams.Count > 0) ? this.IntParams[0] : 0);

            if (team == Team.blue)
            {
                ListTeam.AddRange(managerForBanzai.BlueTeam);
            }
            else if (team == Team.green)
            {
                ListTeam.AddRange(managerForBanzai.GreenTeam);
            }
            else if (team == Team.red)
            {
                ListTeam.AddRange(managerForBanzai.RedTeam);
            }
            else if (team == Team.yellow)
            {
                ListTeam.AddRange(managerForBanzai.YellowTeam);
            }
            else
            {
                return;
            }

            if (ListTeam.Count == 0)
            {
                return;
            }

            foreach (RoomUser teamUser in ListTeam)
            {
                if (teamUser == null)
                {
                    continue;
                }

                this.RoomInstance.GetWiredHandler().TriggerCollision(teamUser, null);
            }
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            int team = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0);

            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, team.ToString(), false, null, this.Delay);
        }

        public void LoadFromDatabase(DataRow row)
        {
            if (int.TryParse(row["delay"].ToString(), out int delay))
	            this.Delay = delay;
                
            if (int.TryParse(row["trigger_data"].ToString(), out int team))
                this.IntParams.Add(team);
        }
    }
}
