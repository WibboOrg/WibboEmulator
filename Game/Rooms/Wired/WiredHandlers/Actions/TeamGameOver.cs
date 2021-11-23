using Butterfly.Communication.Packets.Outgoing.GameCenter;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Games;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Actions
{
    public class TeamGameOver : WiredActionBase, IWired, IWiredEffect
    {
        public TeamGameOver(Item item, Room room) : base(item, room, (int)WiredActionType.JOIN_TEAM)
        {
        }
        
        public void Handle(RoomUser user, Item TriggerItem)
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
                teamuser.Team = Team.none;

                teamuser.GetClient().SendPacket(new IsPlayingComposer(false));

                if (ExitTeleport != null)
                {
                    this.RoomInstance.GetGameMap().TeleportToItem(teamuser, ExitTeleport);
                }
            }
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            int team = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0);

            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, team.ToString(), false, null);
        }

        public void LoadFromDatabase(DataRow row)
        {
            if (int.TryParse(row["trigger_data"].ToString(), out int number))
                this.IntParams.Add(number);
        }
    }
}
