namespace WibboEmulator.Games.Moderations;
using WibboEmulator.Database.Daos.Moderation;
using WibboEmulator.Database.Daos.User;

public class ModerationTicket
{
    public int Id { get; set; }
    public int Score { get; set; }
    public int Type { get; set; }
    public TicketStatusType Status { get; set; }
    public int SenderId { get; set; }
    public int ReportedId { get; set; }
    public int ModeratorId { get; set; }
    public string Message { get; set; }
    public int RoomId { get; set; }
    public string RoomName { get; set; }
    public double Timestamp { get; set; }
    public string SenderName { get; set; }
    public string ReportedName { get; set; }
    public string ModName { get; set; }

    public int TabId
    {
        get
        {
            if (this.Status == TicketStatusType.Open)
            {
                return 1;
            }

            if (this.Status is TicketStatusType.Picked or TicketStatusType.Abusive or TicketStatusType.Invalid or TicketStatusType.Resolved)
            {
                return 2;
            }

            return this.Status == TicketStatusType.Deleted ? 3 : 0;
        }
    }

    public int TicketId => this.Id;

    public ModerationTicket(int id, int score, int type, int senderId, int reportedId, string message, int roomId, string roomName, double timestamp)
    {
        this.Id = id;
        this.Score = score;
        this.Type = type;
        this.Status = TicketStatusType.Open;
        this.SenderId = senderId;
        this.ReportedId = reportedId;
        this.ModeratorId = 0;
        this.Message = message;
        this.RoomId = roomId;
        this.RoomName = roomName;
        this.Timestamp = timestamp;
        this.SenderName = GetNameById(senderId);
        this.ReportedName = GetNameById(reportedId);
        this.ModName = GetNameById(this.ModeratorId);
    }

    public static string GetNameById(int id)
    {
        var username = "";
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            username = UserDao.GetNameById(dbClient, id);
        }

        return username;
    }

    public void Pick(int moderatorId, bool updateInDb)
    {
        this.Status = TicketStatusType.Picked;
        this.ModeratorId = moderatorId;
        this.Timestamp = WibboEnvironment.GetUnixTimestamp();

        if (!updateInDb)
        {
            return;
        }

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        ModerationTicketDao.UpdateStatusPicked(dbClient, moderatorId, this.Id);
    }

    public void Close(TicketStatusType newStatus, bool updateInDb)
    {
        this.Status = newStatus;
        if (!updateInDb)
        {
            return;
        }

        var str = newStatus switch
        {
            TicketStatusType.Abusive => "abusive",
            TicketStatusType.Invalid => "invalid",
            _ => "resolved",
        };
        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        ModerationTicketDao.UpdateStatus(dbClient, str, this.Id);
    }

    public void Release(bool updateInDb)
    {
        this.Status = TicketStatusType.Open;

        if (!updateInDb)
        {
            return;
        }

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        ModerationTicketDao.UpdateStatusOpen(dbClient, this.Id);
    }

    public void Delete(bool updateInDb)
    {
        this.Status = TicketStatusType.Deleted;

        if (!updateInDb)
        {
            return;
        }

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        ModerationTicketDao.UpdateStatusDeleted(dbClient, this.Id);
    }
}
