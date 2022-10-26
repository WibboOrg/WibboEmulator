namespace WibboEmulator.Games.Chat.Logs;

public class ChatlogEntry
{
    public int UserID { get; set; }
    public string Username { get; set; }
    public int RoomID { get; set; }
    public string Message { get; set; }
    public double Timestamp { get; set; }

    public ChatlogEntry(int userID, string username, int roomID, string message, double timestamp)
    {
        this.UserID = userID;
        this.Username = username;
        this.RoomID = roomID;
        this.Message = message;
        this.Timestamp = timestamp;
    }
}
