namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class KickUser : WiredActionBase, IWired, IWiredCycleable, IWiredEffect
{
    public KickUser(Item item, Room room) : base(item, room, (int)WiredActionType.KICK_FROM_ROOM)
    {
    }

    public override bool OnCycle(RoomUser user, Item item)
    {
        if (user != null && user.Client != null)
        {
            if (user.RoomId == this.RoomInstance.RoomData.Id)
            {
                this.RoomInstance.RoomUserManager.RemoveUserFromRoom(user.Client, true, true);
            }
        }

        return false;
    }

    public override void Handle(RoomUser user, Item item)
    {
        if (this.BeforeCycle(user))
        {
            base.Handle(user, item);
        }
    }

    public bool BeforeCycle(RoomUser user)
    {
        if (user != null && user.Client != null && user.Client.User != null)
        {
            if (user.Client.User.HasPermission("mod") || this.RoomInstance.RoomData.OwnerId == user.UserId)
            {
                user.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("wired.kick.exception", user.Client.Langue));

                return false;
            }

            user.ApplyEffect(4);
            user.Freeze = true;

            if (!string.IsNullOrEmpty(this.StringParam))
            {
                user.SendWhisperChat(this.StringParam);
            }
        }

        return true;
    }

    public void SaveToDatabase(IDbConnection dbClient) => WiredUtillity.SaveInDatabase(dbClient, this.Id, string.Empty, this.StringParam, false, null, this.Delay);

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.Delay = wiredDelay;

        this.StringParam = wiredTriggerData;
    }
}
