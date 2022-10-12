namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.GameCenter;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

public class TeamLeave : WiredActionBase, IWired, IWiredEffect
{
    public TeamLeave(Item item, Room room) : base(item, room, (int)WiredActionType.LEAVE_TEAM)
    {
    }

    public override bool OnCycle(RoomUser user, Item item)
    {
        if (user != null && !user.IsBot && user.Client != null && user.Team != TeamType.None && user.Room != null)
        {
            var managerForBanzai = user.Room.GetTeamManager();
            if (managerForBanzai == null)
            {
                return false;
            }

            managerForBanzai.OnUserLeave(user);
            user.Room.GetGameManager().UpdateGatesTeamCounts();
            user.ApplyEffect(0);
            user.Team = TeamType.None;

            user.Client.SendPacket(new IsPlayingComposer(false));
        }

        return true;
    }

    public void SaveToDatabase(IQueryAdapter dbClient) => WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, string.Empty, false, null, this.Delay);

    public void LoadFromDatabase(DataRow row)
    {
        if (int.TryParse(row["delay"].ToString(), out var delay))
        {
            this.Delay = delay;
        }
    }
}
