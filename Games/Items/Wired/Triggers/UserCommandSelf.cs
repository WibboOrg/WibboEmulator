namespace WibboEmulator.Games.Items.Wired.Triggers;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Events;

public class UserCommandSelf : WiredTriggerBase, IWired
{
    public UserCommandSelf(Item item, Room room) : base(item, room, (int)WiredTriggerType.AVATAR_SAYS_COMMAND) => room.OnCommandSelf += this.OnUserSays;

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

        if (!message.Contains(' ') || !message.StartsWith(':'))
        {
            return;
        }

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

        if (!messageCommand.Equals(commandName, StringComparison.CurrentCultureIgnoreCase))
        {
            return;
        }

        e.Result = true;

        var TargetUser = this.Room.RoomUserManager.GetRoomUserByName(messageUserName);
        TargetUser ??= this.Room.RoomUserManager.GetBotOrPetByName(messageUserName);

        if (TargetUser == null)
        {
            return;
        }

        if (TargetUser == user)
        {
            return;
        }

        if (Math.Abs(TargetUser.X - user.X) >= distance || Math.Abs(TargetUser.Y - user.Y) >= distance)
        {
            return;
        }

        this.Room.WiredHandler.ExecutePile(this.Item.Coordinate, user, null);
    }

    public override void Dispose()
    {
        base.Dispose();

        this.Room.OnCommandSelf -= this.OnUserSays;
    }

    public void SaveToDatabase(IDbConnection dbClient) => WiredUtillity.SaveInDatabase(dbClient, this.Id, string.Empty, this.StringParam);

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay) => this.StringParam = wiredTriggerData == "" ? "trigger:1" : wiredTriggerData;
}
