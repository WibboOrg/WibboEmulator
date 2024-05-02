namespace WibboEmulator.Games.Items.Wired.Conditions;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

public class ActorNotInTeam : WiredConditionBase, IWiredCondition, IWired
{
    public ActorNotInTeam(Item item, Room room) : base(item, room, (int)WiredConditionType.NOT_ACTOR_IN_TEAM) => this.DefaultIntParams((int)TeamType.Red);

    public bool AllowsExecution(RoomUser user, Item item)
    {
        if (user == null)
        {
            return false;
        }

        var teamId = this.GetIntParam(0);
        if (teamId is < 1 or > 4)
        {
            teamId = 1;
        }

        if (user.Team == (TeamType)teamId)
        {
            return false;
        }

        return true;
    }

    public void SaveToDatabase(IDbConnection dbClient)
    {
        var teamId = this.GetIntParam(0);

        WiredUtillity.SaveInDatabase(dbClient, this.Id, string.Empty, teamId.ToString());
    }

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        if (int.TryParse(wiredTriggerData, out var teamId))
        {
            this.SetIntParam(0, teamId);
        }
    }
}
