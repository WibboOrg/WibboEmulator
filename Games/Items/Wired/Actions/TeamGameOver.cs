namespace WibboEmulator.Games.Items.Wired.Actions;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;
using WibboEmulator.Games.Rooms.Map;

public class TeamGameOver : WiredActionBase, IWired, IWiredEffect
{
    public TeamGameOver(Item item, Room room) : base(item, room, (int)WiredActionType.JOIN_TEAM) => this.DefaultIntParams(new int[] { (int)TeamType.Red });

    public override bool OnCycle(RoomUser user, Item item)
    {
        var managerForBanzai = this.RoomInstance.TeamManager;

        var listTeam = new List<RoomUser>();

        var team = (TeamType)this.GetIntParam(0);

        if (team == TeamType.Blue)
        {
            listTeam.AddRange(managerForBanzai.BlueTeam);
        }
        else if (team == TeamType.Green)
        {
            listTeam.AddRange(managerForBanzai.GreenTeam);
        }
        else if (team == TeamType.Red)
        {
            listTeam.AddRange(managerForBanzai.RedTeam);
        }
        else if (team == TeamType.Yellow)
        {
            listTeam.AddRange(managerForBanzai.YellowTeam);
        }
        else
        {
            return false;
        }

        var exitTeleport = this.RoomInstance.GameItemHandler.GetExitTeleport();

        foreach (var teamuser in listTeam)
        {
            if (teamuser == null)
            {
                continue;
            }

            managerForBanzai.OnUserLeave(teamuser);
            this.RoomInstance.GameManager.UpdateGatesTeamCounts();
            teamuser.ApplyEffect(0);
            teamuser.Team = TeamType.None;

            teamuser.Client.SendPacket(new IsPlayingComposer(false));

            if (exitTeleport != null)
            {
                GameMap.TeleportToItem(teamuser, exitTeleport);
            }
        }

        return false;
    }

    public void SaveToDatabase(IDbConnection dbClient)
    {
        var team = this.GetIntParam(0);

        WiredUtillity.SaveInDatabase(dbClient, this.Id, string.Empty, team.ToString(), false, null, this.Delay);
    }

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.Delay = wiredDelay;

        if (int.TryParse(wiredTriggerData, out var team))
        {
            this.SetIntParam(0, team);
        }
    }
}
