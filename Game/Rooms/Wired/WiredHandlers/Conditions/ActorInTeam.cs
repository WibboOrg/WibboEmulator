using Butterfly.Database.Interfaces;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Games;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Conditions
{
    public class ActorInTeam : WiredBase, IWiredCondition, IWired
    {
        private bool isDisposed;

        public ActorInTeam(int itemid, int teamId)
        {
            if (teamId < 1 || teamId > 4)
            {
                teamId = 1;
            }

            this.Id = itemid;

            this.IntParams.Add(teamId);
            this.isDisposed = false;
        }

        public bool AllowsExecution(RoomUser user, Item TriggerItem)
        {
            if (user == null)
            {
                return false;
            }

            if(this.IntParams.Count == 0)
            {
                return false;
            }

            if (user.Team != (Team)this.IntParams[0])
            {
                return false;
            }

            return true;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            if (this.IntParams.Count == 0)
            {
                return;
            }

            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, this.IntParams[0].ToString(), false, null);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            if (int.TryParse(row["trigger_data"].ToString(), out int teamId))
            {
                this.IntParams.Add(teamId);
            }
        }

        /*public void OnTrigger(Client Session, int SpriteId)
        {
            Session.SendPacket(new WiredFurniConditionMessageComposer(false, 0, null, SpriteId, this.Id, "", new List<int> { (int)this.team }, 0, (int)WiredConditionType.ACTOR_IS_IN_TEAM));
        }*/

        public void Dispose()
        {
            this.isDisposed = true;
        }

        public bool Disposed()
        {
            return this.isDisposed;
        }
    }
}
