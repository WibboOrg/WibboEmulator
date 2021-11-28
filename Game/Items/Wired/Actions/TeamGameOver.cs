using Butterfly.Communication.Packets.Outgoing.GameCenter;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Rooms;
using Butterfly.Game.Rooms.Games;
using Butterfly.Game.Items.Wired.Interfaces;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Items.Wired.Actions
{
    public class TeamGameOver : WiredActionBase, IWired, IWiredEffect
    {
        public TeamGameOver(Item item, Room room) : base(item, room, (int)WiredActionType.JOIN_TEAM)
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
            TeamManager managerForBanzai = this.RoomInstance.GetTeamManager();

            List<RoomUser> ListTeam = new List<RoomUser>();

            TeamType team = (TeamType)((this.IntParams.Count > 0) ? this.IntParams[0] : 0);

            if (team == TeamType.blue)
            {
                ListTeam.AddRange(managerForBanzai.BlueTeam);
            }
            else if (team == TeamType.green)
            {
                ListTeam.AddRange(managerForBanzai.GreenTeam);
            }
            else if (team == TeamType.red)
            {
                ListTeam.AddRange(managerForBanzai.RedTeam);
            }
            else if (team == TeamType.yellow)
            {
                ListTeam.AddRange(managerForBanzai.YellowTeam);
            }
            else
            {
                return false;
            }

            Item ExitTeleport = this.RoomInstance.GetGameItemHandler().GetExitTeleport();

            foreach (RoomUser teamuser in ListTeam)
            {
                if (teamuser == null)
                {
                    continue;
                }

                managerForBanzai.OnUserLeave(teamuser);
                this.RoomInstance.GetGameManager().UpdateGatesTeamCounts();
                teamuser.ApplyEffect(0);
                teamuser.Team = TeamType.none;

                teamuser.GetClient().SendPacket(new IsPlayingComposer(false));

                if (ExitTeleport != null)
                {
                    this.RoomInstance.GetGameMap().TeleportToItem(teamuser, ExitTeleport);
                }
            }

            return false;
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
                
            if (int.TryParse(row["trigger_data"].ToString(), out int number))
                this.IntParams.Add(number);
        }
    }
}
