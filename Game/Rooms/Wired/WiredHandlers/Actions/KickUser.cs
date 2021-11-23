using Butterfly.Database.Interfaces;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Actions
{
    public class KickUser : WiredActionBase, IWired, IWiredCycleable, IWiredEffect
    {   
        public KickUser(Item item, Room room) : base(item, room, (int)WiredActionType.KICK_FROM_ROOM)
        {
        }

        public bool OnCycle(RoomUser user, Item item)
        {
            if (user != null && user.GetClient() != null)
            {
                if (user.RoomId == this.RoomInstance.RoomData.Id)
                {
                    this.RoomInstance.GetRoomUserManager().RemoveUserFromRoom(user.GetClient(), true, true);
                }
            }
            
            return false;
        }

        public void Handle(RoomUser User, Item TriggerItem)
        {
            if (User != null && User.GetClient() != null && User.GetClient().GetHabbo() != null)
            {
                if (User.GetClient().GetHabbo().HasFuse("fuse_mod") || this.RoomInstance.RoomData.OwnerId == User.UserId)
                {
                    User.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("wired.kick.exception", User.GetClient().Langue));
                    
                    return;
                }

                User.ApplyEffect(4);
                User.Freeze = true;

                if (!string.IsNullOrEmpty(this.StringParam))
                {
                    User.SendWhisperChat(this.StringParam);
                }

                this.RoomInstance.GetWiredHandler().RequestCycle(new WiredCycle(this, User, null, this.DelayCycle));
            }
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, this.StringParam, false, null);
        }

        public void LoadFromDatabase(DataRow row)
        {
            this.StringParam = row["trigger_data"].ToString();
        }
    }
}
