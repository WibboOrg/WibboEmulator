namespace WibboEmulator.Games.Users.Messenger;

public class MessengerBuddy
{
    public int UserId { get; }
    public string Username { get; }
    public string Look { get; private set; }
    public int Relation { get; private set; }
    public bool IsOnline { get; private set; }
    public bool HideInRoom { get; private set; }

    public MessengerBuddy(int userId, string username, string look, int relation)
    {
        this.UserId = userId;
        this.Username = username;
        this.Look = look;
        this.Relation = relation;
    }

    public void UpdateRelation(int type) => this.Relation = type;

    public void UpdateUser()
    {
        var client = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(this.UserId);
        if (client != null && client.GetUser() != null && client.GetUser().Messenger != null && !client.GetUser().Messenger.AppearOffline)
        {
            this.IsOnline = true;
            this.Look = client.GetUser().Look;
            this.HideInRoom = client.GetUser().HideInRoom;
        }
        else
        {
            this.IsOnline = false;
            this.Look = "";
            this.HideInRoom = true;
        }
    }
}
