using Butterfly.Communication.Packets.Outgoing.GameCenter;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Games;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Actions
{
    public class TeamLeave : WiredActionBase, IWired, IWiredEffect
    {
        public TeamLeave(Item item, Room room) : base(item, room, (int)WiredActionType.LEAVE_TEAM)
        {
        }

        public override bool OnCycle(RoomUser user, Item item)
        {
            if (user != null && !user.IsBot && user.GetClient() != null && user.Team != Team.none && user.Room != null)
            {
                TeamManager managerForBanzai = user.Room.GetTeamManager();
                if (managerForBanzai == null)
                {
                    return false;
                }

                managerForBanzai.OnUserLeave(user);
                user.Room.GetGameManager().UpdateGatesTeamCounts();
                user.ApplyEffect(0);
                user.Team = Team.none;

                user.GetClient().SendPacket(new IsPlayingComposer(false));
            }

            return true;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, string.Empty, false, null, this.Delay);
        }

        public void LoadFromDatabase(DataRow row)
        {
            if (int.TryParse(row["delay"].ToString(), out int delay))
	            this.Delay = delay;
        }
    }
}
