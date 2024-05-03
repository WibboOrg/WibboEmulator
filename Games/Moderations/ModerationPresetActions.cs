namespace WibboEmulator.Games.Moderations;

public class ModerationPresetActions(int id, int parentId, string type, string caption, string messageText, int muteText, int banTime, int ipBanTime, int tradeLockTime, string defaultSanction)
{
    public int Id { get; private set; } = id;
    public int ParentId { get; private set; } = parentId;
    public string Type { get; private set; } = type;
    public string Caption { get; private set; } = caption;
    public string MessageText { get; private set; } = messageText;
    public int MuteTime { get; private set; } = muteText;
    public int BanTime { get; private set; } = banTime;
    public int IPBanTime { get; private set; } = ipBanTime;
    public int TradeLockTime { get; private set; } = tradeLockTime;
    public string DefaultSanction { get; private set; } = defaultSanction;
}
