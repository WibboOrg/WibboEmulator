namespace WibboEmulator.Games.Chats.Logs;

public class ChatlogEntry(int userID, string username, int roomID, string message, double timestamp)
{
    public int UserID { get; set; } = userID;
    public string Username { get; set; } = username;
    public int RoomID { get; set; } = roomID;
    public string Message { get; set; } = message;
    public double Timestamp { get; set; } = timestamp;
}
