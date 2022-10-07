namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.GameCenter;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;
using WibboEmulator.Games.Rooms.Map;

public class TeamGameOver : WiredActionBase, IWired, IWiredEffect
{
    public TeamGameOver(Item item, Room room) : base(item, room, (int)WiredActionType.JOIN_TEAM) => this.IntParams.Add((int)TeamType.RED);

    public override bool OnCycle(RoomUser user, Item item)
    {
        var managerForBanzai = this.RoomInstance.GetTeamManager();

        var listTeam = new List<RoomUser>();

        var team = (TeamType)((this.IntParams.Count > 0) ? this.IntParams[0] : 0);

        if (team == TeamType.BLUE)
        {
            listTeam.AddRange(managerForBanzai.BlueTeam);
        }
        else if (team == TeamType.GREEN)
        {
            listTeam.AddRange(managerForBanzai.GreenTeam);
        }
        else if (team == TeamType.RED)
        {
            listTeam.AddRange(managerForBanzai.RedTeam);
        }
        else if (team == TeamType.YELLOW)
        {
            listTeam.AddRange(managerForBanzai.YellowTeam);
        }
        else
        {
            return false;
        }

        var exitTeleport = this.RoomInstance.GetGameItemHandler().GetExitTeleport();

        foreach (var teamuser in listTeam)
        {
            if (teamuser == null)
            {
                continue;
            }

            managerForBanzai.OnUserLeave(teamuser);
            this.RoomInstance.GetGameManager().UpdateGatesTeamCounts();
            teamuser.ApplyEffect(0);
            teamuser.Team = TeamType.NONE;

            teamuser.GetClient().SendPacket(new IsPlayingComposer(false));

            if (exitTeleport != null)
            {
                Gamemap.TeleportToItem(teamuser, exitTeleport);
            }
        }

        return false;
    }

    public void SaveToDatabase(IQueryAdapter dbClient)
    {
        var team = (this.IntParams.Count > 0) ? this.IntParams[0] : 0;

        WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, team.ToString(), false, null, this.Delay);
    }

    public void LoadFromDatabase(DataRow row)
    {
        this.IntParams.Clear();

        if (int.TryParse(row["delay"].ToString(), out var delay))
        {
            this.Delay = delay;
        }

        if (int.TryParse(row["trigger_data"].ToString(), out var number))
        {
            this.IntParams.Add(number);
        }
    }
}
