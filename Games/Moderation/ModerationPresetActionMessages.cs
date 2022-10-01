namespace WibboEmulator.Games.Moderation
{
    public class ModerationPresetActionMessages
    {
        public string Title { get; private set; }
        public string SubTitle { get; private set; }
        public int BanHours { get; private set; }
        public int Enablemute { get; private set; }
        public int MuteHours { get; private set; }
        public int Reminder { get; private set; }
        public string Message { get; private set; }

        public ModerationPresetActionMessages(string title, string subTitle, int banHours, int enablemute, int muteHours, int reminder, string message)
        {
            this.Title = title;
            this.SubTitle = subTitle;
            this.BanHours = banHours;
            this.Enablemute = enablemute;
            this.MuteHours = muteHours;
            this.Reminder = reminder;
            this.Message = message;
        }
    }
}