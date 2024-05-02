namespace WibboEmulator.Games.Items.Wired.Triggers;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Events;

public class UserSays : WiredTriggerBase, IWired
{
    public UserSays(Item item, Room room) : base(item, room, (int)WiredTriggerType.AVATAR_SAYS_SOMETHING)
    {
        this.DefaultIntParams(0, 0);

        room.OnUserSays += this.OnUserSays;
    }

    private void OnUserSays(object sender, UserSaysEventArgs args)
    {
        var user = args.User;
        var message = args.Message;

        var isOwnerOnly = this.GetIntParam(0) == 1;
        var isContains = this.GetIntParam(1) == 1;

        if (user == null)
        {
            return;
        }

        if ((!isOwnerOnly && this.CanBeTriggered(message, isContains) && !string.IsNullOrEmpty(message)) || (isOwnerOnly && user.IsOwner && this.CanBeTriggered(message, isContains) && !string.IsNullOrEmpty(message)))
        {
            this.Room.WiredHandler.ExecutePile(this.Item.Coordinate, user, null);
            args.Result = true;
        }
    }

    private bool CanBeTriggered(string message, bool isContains)
    {
        if (string.IsNullOrEmpty(this.StringParam))
        {
            return false;
        }

        if (isContains)
        {
            return message.ToLower().Contains(this.StringParam.ToLower());
        }

        return message.ToLower() == this.StringParam.ToLower();
    }

    public override void Dispose()
    {
        base.Dispose();

        this.Room.OnUserSays -= this.OnUserSays;
    }

    public void SaveToDatabase(IDbConnection dbClient)
    {
        var isOwnerOnly = this.GetIntParam(0) == 1;
        var isContains = this.GetIntParam(1) == 1;

        WiredUtillity.SaveInDatabase(dbClient, this.Id, isContains ? "1" : "0", this.StringParam, isOwnerOnly);
    }

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.StringParam = wiredTriggerData;

        this.SetIntParam(0, wiredAllUserTriggerable ? 1 : 0);
        this.SetIntParam(1, wiredTriggerData2 == "1" ? 1 : 0);
    }
}
