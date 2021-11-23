using Butterfly.Communication.Packets.Outgoing.GameCenter;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Games;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Actions
{
    public class TeamJoin : WiredActionBase, IWired, IWiredEffect
    {
        public TeamJoin(Item item, Room room) : base(item, room, (int)WiredActionType.JOIN_TEAM)
        {
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (user != null && !user.IsBot && user.GetClient() != null && user.Room != null)
            {
                TeamManager managerForFreeze = user.Room.GetTeamManager();

                if (user.Team != Team.none)
                {
                    managerForFreeze.OnUserLeave(user);
                }
                
                Team team = (Team)((this.IntParams.Count > 0) ? this.IntParams[0] : 0);

                user.Team = team;
                managerForFreeze.AddUser(user);
                user.Room.GetGameManager().UpdateGatesTeamCounts();

                int EffectId = ((int)team + 39);
                user.ApplyEffect(EffectId);

                user.GetClient().SendPacket(new IsPlayingComposer(true));
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
