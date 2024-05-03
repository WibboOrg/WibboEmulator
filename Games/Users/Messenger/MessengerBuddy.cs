namespace WibboEmulator.Games.Users.Messenger;

using WibboEmulator.Games.GameClients;

public class MessengerBuddy(int userId, string username, string look, int relation)
{
    public int UserId { get; } = userId;
    public string Username { get; } = username;
    public string Look { get; private set; } = look;
    public int Relation { get; private set; } = relation;
    public bool IsOnline { get; private set; }
    public bool HideInRoom { get; private set; }

    public void UpdateRelation(int type) => this.Relation = type;

    public void UpdateUser()
    {
        var client = GameClientManager.GetClientByUserID(this.UserId);
        if (client != null && client.User != null && client.User.Messenger != null && !client.User.Messenger.AppearOffline)
        {
            this.IsOnline = true;
            this.Look = client.User.Look;
            this.HideInRoom = client.User.HideInRoom;
        }
        else
        {
            this.IsOnline = false;
            this.Look = "";
            this.HideInRoom = true;
        }
    }
}
