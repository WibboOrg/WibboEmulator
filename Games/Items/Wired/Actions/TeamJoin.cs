using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.GameCenter;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games;

namespace WibboEmulator.Games.Items.Wired.Actions
{
    public class TeamJoin : WiredActionBase, IWired, IWiredEffect
    {
        public TeamJoin(Item item, Room room) : base(item, room, (int)WiredActionType.JOIN_TEAM)
        {
            this.IntParams.Add((int)TeamType.RED);
        }

        public override bool OnCycle(RoomUser user, Item item)
        {
            if (user != null && !user.IsBot && user.GetClient() != null && user.Room != null)
            {
                TeamManager managerForFreeze = user.Room.GetTeamManager();

                if (user.Team != TeamType.NONE)
                {
                    managerForFreeze.OnUserLeave(user);
                }

                TeamType team = (TeamType)((this.IntParams.Count > 0) ? this.IntParams[0] : 0);

                user.Team = team;
                managerForFreeze.AddUser(user);
                user.Room.GetGameManager().UpdateGatesTeamCounts();

                int EffectId = ((int)team + 39);
                user.ApplyEffect(EffectId);

                user.GetClient().SendPacket(new IsPlayingComposer(true));
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
            this.IntParams.Clear();

            if (int.TryParse(row["delay"].ToString(), out int delay))
                this.Delay = delay;

            if (int.TryParse(row["trigger_data"].ToString(), out int number))
                this.IntParams.Add(number);
        }
    }
}
