namespace WibboEmulator.Games.Items.Wired.Triggers;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Utilities.Events;

public class UserSays : WiredTriggerBase, IWired
{
    public UserSays(Item item, Room room) : base(item, room, (int)WiredTriggerType.AVATAR_SAYS_SOMETHING)
    {
        room.OnUserSays += this.OnUserSays;

        this.IntParams.Add(0);
    }

    private void OnUserSays(object sender, UserSaysEventArgs args)
    {
        var user = args.User;
        var message = args.Message;

        var isOwnerOnly = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0) == 1;

        if (user == null)
        {
            return;
        }

        if ((!isOwnerOnly && this.CanBeTriggered(message) && !string.IsNullOrEmpty(message)) || (isOwnerOnly && user.IsOwner() && this.CanBeTriggered(message) && !string.IsNullOrEmpty(message)))
        {
            this.RoomInstance.WiredHandler.ExecutePile(this.ItemInstance.Coordinate, user, null);
            args.Result = true;
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

        this.RoomInstance.OnUserSays -= this.OnUserSays;
    }

    public void SaveToDatabase(IQueryAdapter dbClient)
    {
        var isOwnerOnly = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0) == 1;

        WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, this.StringParam, isOwnerOnly, null);
    }

    public void LoadFromDatabase(DataRow row)
    {
        this.IntParams.Clear();

        this.StringParam = row["trigger_data"].ToString();

        if (bool.TryParse(row["all_user_triggerable"].ToString(), out var isOwnerOnly))
        {
            this.IntParams.Add(isOwnerOnly ? 1 : 0);
        }
    }
}
