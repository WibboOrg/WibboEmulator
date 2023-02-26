namespace WibboEmulator.Games.Items.Wired.Triggers;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Events;

public class UserTriggerSelf : WiredTriggerBase, IWired
{
    public UserTriggerSelf(Item item, Room room) : base(item, room, (int)WiredTriggerType.AVATAR_SAYS_COMMAND) => room.OnTriggerSelf += this.OnUserSays;

    private void OnUserSays(object sender, UserSaysEventArgs e)
    {
        if (!this.StringParam.Contains(':'))
        {
            this.StringParam = "trigger:1";
        }

        var parts = this.StringParam.Split(':');
        if (parts.Length != 2)
        {
            return;
        }

        var commandName = parts[0];
        var commandValue = parts[1];

        var user = e.User;
        var message = e.Message;
        if (user == null || user.IsBot)
        {
            return;
        }

        if (!int.TryParse(commandValue, out var distance))
        {
            return;
        }

        distance += 1;

        var messageParts = message[1..].Split(' ');

        var messageCommand = messageParts[0];
        var messageUserName = messageParts[1];

        if (messageCommand.ToLower() != commandName.ToLower())
        {
            return;
        }

        e.Result = true;

        var targetUser = this.RoomInstance.RoomUserManager.GetRoomUserByName(messageUserName);
        targetUser ??= this.RoomInstance.RoomUserManager.GetBotOrPetByName(messageUserName);

        if (targetUser == null)
        {
            return;
        }

        if (targetUser == user)
        {
            return;
        }

        if (Math.Abs(targetUser.X - user.X) >= distance || Math.Abs(targetUser.Y - user.Y) >= distance)
        {
            return;
        }

        this.RoomInstance.WiredHandler.ExecutePile(this.ItemInstance.Coordinate, user, null);
    }

    public override void Dispose()
    {
        base.Dispose();

        this.RoomInstance.OnTriggerSelf -= this.OnUserSays;
    }

    public void SaveToDatabase(IQueryAdapter dbClient) => WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, this.StringParam, false, null);

    public void LoadFromDatabase(DataRow row)
    {
        var triggerData = row["trigger_data"].ToString();

        this.StringParam = triggerData == "" ? "trigger:1" : triggerData;
    }
}
