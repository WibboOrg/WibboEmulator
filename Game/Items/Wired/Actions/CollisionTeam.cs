using Butterfly.Database.Interfaces;
using Butterfly.Game.Rooms;
using Butterfly.Game.Rooms.Games;
using Butterfly.Game.Items.Wired.Interfaces;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Items.Wired.Actions
{
    public class CollisionTeam : WiredActionBase, IWiredEffect, IWired
    {
        public CollisionTeam(Item item, Room room) : base(item, room, (int)WiredActionType.JOIN_TEAM)
        {
        }

        public override void LoadItems(bool inDatabase = false)
        {
            base.LoadItems(inDatabase);

            if (inDatabase)
                return;

            this.IntParams.Add((int)TeamType.red);
        }

        public override bool OnCycle(RoomUser user, Item item)
        {
            this.HandleItems();

            return false;
        }

        private void HandleItems()
        {
            TeamManager managerForBanzai = this.RoomInstance.GetTeamManager();

            List<RoomUser> listTeam = new List<RoomUser>();

            TeamType team = (TeamType)((this.IntParams.Count > 0) ? this.IntParams[0] : 0);

            if (team == TeamType.blue)
            {
                listTeam.AddRange(managerForBanzai.BlueTeam);
            }
            else if (team == TeamType.green)
            {
                listTeam.AddRange(managerForBanzai.GreenTeam);
            }
            else if (team == TeamType.red)
            {
                listTeam.AddRange(managerForBanzai.RedTeam);
            }
            else if (team == TeamType.yellow)
            {
                listTeam.AddRange(managerForBanzai.YellowTeam);
            }
            else
            {
                return;
            }

            if (listTeam.Count == 0)
            {
                return;
            }

            foreach (RoomUser teamUser in listTeam)
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
