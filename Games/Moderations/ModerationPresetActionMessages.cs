namespace WibboEmulator.Games.Moderations;

public class ModerationPresetActionMessages(string title, string subTitle, int banHours, int enablemute, int muteHours, int reminder, string message)
{
    public string Title { get; private set; } = title;
    public string SubTitle { get; private set; } = subTitle;
    public int BanHours { get; private set; } = banHours;
    public int Enablemute { get; private set; } = enablemute;
    public int MuteHours { get; private set; } = muteHours;
    public int Reminder { get; private set; } = reminder;
    public string Message { get; private set; } = message;
}