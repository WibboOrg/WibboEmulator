namespace WibboEmulator.Games.Items.Wired.Actions;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

public class TeamJoin : WiredActionBase, IWired, IWiredEffect
{
    public TeamJoin(Item item, Room room) : base(item, room, (int)WiredActionType.JOIN_TEAM) => this.DefaultIntParams(new int[] { (int)TeamType.Red });

    public override bool OnCycle(RoomUser user, Item item)
    {
        if (user != null && !user.IsBot && user.Client != null && user.Room != null)
        {
            var managerForFreeze = user.Room.TeamManager;

            if (user.Team != TeamType.None)
            {
                managerForFreeze.OnUserLeave(user);
            }

            var team = (TeamType)this.GetIntParam(0);

            user.Team = team;
            managerForFreeze.AddUser(user);
            user.Room.GameManager.UpdateGatesTeamCounts();

            var effectId = (int)team + 39;
            user.ApplyEffect(effectId);

            user.Client.SendPacket(new IsPlayingComposer(true));
        }

        return false;
    }

    public void SaveToDatabase(IDbConnection dbClient)
    {
        var team = this.GetIntParam(0);

        WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, team.ToString(), false, null, this.Delay);
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
