namespace WibboEmulator.Games.Items.Wired.Triggers;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Utilities.Events;

public class UserSays : WiredTriggerBase, IWired
{
    private readonly RoomUserSaysDelegate _delegateFunction;

    public UserSays(Item item, Room room) : base(item, room, (int)WiredTriggerType.AVATAR_SAYS_SOMETHING)
    {
        this._delegateFunction = new RoomUserSaysDelegate(this.OnUserSays);
        room.OnUserSays += this._delegateFunction;

        this.IntParams.Add(0);
    }

    private void OnUserSays(object sender, UserSaysEventArgs e, ref bool messageHandled)
    {
        var user = e.User;
        var message = e.Message;

        var isOwnerOnly = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0) == 1;

        if (user == null)
        {
            return;
        }

        if ((!isOwnerOnly && this.CanBeTriggered(message) && !string.IsNullOrEmpty(message)) || (isOwnerOnly && user.IsOwner() && this.CanBeTriggered(message) && !string.IsNullOrEmpty(message)))
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

        this.RoomInstance.GetWiredHandler().GetRoom().OnUserSays -= this._delegateFunction;
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
