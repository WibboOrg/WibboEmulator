namespace WibboEmulator.Games.Moderation;
using WibboEmulator.Database.Daos.Moderation;
using WibboEmulator.Database.Daos.User;

public class ModerationTicket
{
    public int Id;
    public int Score;
    public int Type;
    public TicketStatusType Status;
    public int SenderId;
    public int ReportedId;
    public int ModeratorId;
    public string Message;
    public int RoomId;
    public string RoomName;
    public double Timestamp;
    public string SenderName;
    public string ReportedName;
    public string ModName;

    public int TabId
    {
        get
        {
            if (this.Status == TicketStatusType.OPEN)
            {
                return 1;
            }

            if (this.Status is TicketStatusType.PICKED or TicketStatusType.ABUSIVE or TicketStatusType.INVALID or TicketStatusType.RESOLVED)
            {
                return 2;
            }

            return this.Status == TicketStatusType.DELETED ? 3 : 0;
        }
    }

    public int TicketId => this.Id;

    public ModerationTicket(int Id, int Score, int Type, int SenderId, int ReportedId, string Message, int RoomId, string RoomName, double Timestamp)
    {
        this.Id = Id;
        this.Score = Score;
        this.Type = Type;
        this.Status = TicketStatusType.OPEN;
        this.SenderId = SenderId;
        this.ReportedId = ReportedId;
        this.ModeratorId = 0;
        this.Message = Message;
        this.RoomId = RoomId;
        this.RoomName = RoomName;
        this.Timestamp = Timestamp;
        this.SenderName = this.GetNameById(SenderId);
        this.ReportedName = this.GetNameById(ReportedId);
        this.ModName = this.GetNameById(this.ModeratorId);
    }

    public string GetNameById(int Id)
    {
        var username = "";
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            username = UserDao.GetNameById(dbClient, Id);
        }

        return username;
    }

    public void Pick(int moderatorId, bool UpdateInDb)
    {
        this.Status = TicketStatusType.PICKED;
        this.ModeratorId = moderatorId;
        this.Timestamp = WibboEnvironment.GetUnixTimestamp();

        if (!UpdateInDb)
        {
            return;
        }

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        ModerationTicketDao.UpdateStatusPicked(dbClient, moderatorId, this.Id);
    }

    public void Close(TicketStatusType NewStatus, bool UpdateInDb)
    {
        this.Status = NewStatus;
        if (!UpdateInDb)
        {
            return;
        }

        string str;
        switch (NewStatus)
        {
            case TicketStatusType.ABUSIVE:
                str = "abusive";
                break;
            case TicketStatusType.INVALID:
                str = "invalid";
                break;
            default:
                str = "resolved";
                break;
        }

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        ModerationTicketDao.UpdateStatus(dbClient, str, this.Id);
    }

    public void Release(bool UpdateInDb)
    {
        this.Status = TicketStatusType.OPEN;

        if (!UpdateInDb)
        {
            return;
        }

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        ModerationTicketDao.UpdateStatusOpen(dbClient, this.Id);
    }

    public void Delete(bool UpdateInDb)
    {
        this.Status = TicketStatusType.DELETED;

        if (!UpdateInDb)
        {
            return;
        }

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        ModerationTicketDao.UpdateStatusDeleted(dbClient, this.Id);
    }
}
