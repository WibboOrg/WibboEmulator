using Wibbo.Database.Interfaces;
using Wibbo.Game.Rooms;
using Wibbo.Game.Items.Wired.Interfaces;
using System.Data;
using Wibbo.Utilities.Events;

namespace Wibbo.Game.Items.Wired.Triggers
{
    public class UserSays : WiredTriggerBase, IWired
    {
        private readonly RoomUserSaysDelegate delegateFunction;

        public UserSays(Item item, Room room) : base(item, room, (int)WiredTriggerType.AVATAR_SAYS_SOMETHING)
        {
            this.delegateFunction = new RoomUserSaysDelegate(this.OnUserSays);
            room.OnUserSays += this.delegateFunction;

            this.IntParams.Add(0);
        }

        private void OnUserSays(object sender, UserSaysArgs e, ref bool messageHandled)
        {
            RoomUser user = e.User;
            string message = e.Message;

            bool isOwnerOnly = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0) == 1;

            if (user != null && (!isOwnerOnly && this.CanBeTriggered(message) && !string.IsNullOrEmpty(message)) || (isOwnerOnly && user.IsOwner() && this.CanBeTriggered(message) && !string.IsNullOrEmpty(message)))
            {
                this.RoomInstance.GetWiredHandler().ExecutePile(this.ItemInstance.Coordinate, user, null);
                messageHandled = true;
            }
        }

        private bool CanBeTriggered(string message)
        {
            if (string.IsNullOrEmpty(this.StringParam))
            {
                return false;
            }

            return message.ToLower() == this.StringParam.ToLower();
        }

        public override void Dispose()
        {
            base.Dispose();

            this.RoomInstance.GetWiredHandler().GetRoom().OnUserSays -= this.delegateFunction;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            bool isOwnerOnly = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0) == 1;

            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, this.StringParam, isOwnerOnly, null);
        }

        public void LoadFromDatabase(DataRow row)
        {
            this.IntParams.Clear();

            this.StringParam = row["trigger_data"].ToString();
            
            if (bool.TryParse(row["all_user_triggerable"].ToString(), out bool isOwnerOnly))
	            this.IntParams.Add(isOwnerOnly ? 1 : 0);
        }
    }
}
