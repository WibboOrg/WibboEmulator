namespace WibboEmulator.Games.Items.Wired.Triggers;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Events;

public class UserCommand : WiredTriggerBase, IWired
{
    public UserCommand(Item item, Room room) : base(item, room, (int)WiredTriggerType.AVATAR_SAYS_COMMAND) => room.OnCommandTarget += this.OnUserSays;

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
            distance = 1;
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

        this.RoomInstance.WiredHandler.ExecutePile(this.ItemInstance.Coordinate, targetUser, null);
    }

    public override void Dispose()
    {
        base.Dispose();

        this.RoomInstance.OnCommandTarget -= this.OnUserSays;
    }

    public void SaveToDatabase(IDbConnection dbClient) => WiredUtillity.SaveInDatabase(dbClient, this.Id, string.Empty, this.StringParam);

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay) => this.StringParam = wiredTriggerData == "" ? "trigger:1" : wiredTriggerData;
}
