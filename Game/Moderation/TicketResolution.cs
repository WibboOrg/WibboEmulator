namespace Butterfly.Game.Moderation
{
    public class TicketResolution
    {
        public string Title;
        public string SubTitle;
        public int BanHours;
        public int Enablemute;
        public int MuteHours;
        public int Reminder;
        public string Message;

        public TicketResolution(string Title, string SubTitle, int BanHours, int enablemute, int MuteHours, int Reminder, string Message)
        {
            this.Title = Title;
            this.SubTitle = SubTitle;
            this.BanHours = BanHours;
            this.Enablemute = enablemute;
            this.MuteHours = MuteHours;
            this.Reminder = Reminder;
            this.Message = Message;
        }
    }
}