namespace WibboEmulator.Game.Moderation
{
    public class ModerationPresetActions
    {
        public int Id { get; private set; }
        public int ParentId { get; private set; }
        public string Type { get; private set; }
        public string Caption { get; private set; }
        public string MessageText { get; private set; }
        public int MuteTime { get; private set; }
        public int BanTime { get; private set; }
        public int IPBanTime { get; private set; }
        public int TradeLockTime { get; private set; }
        public string DefaultSanction { get; private set; }

        public ModerationPresetActions(int id, int parentId, string type, string caption, string messageText, int muteText, int banTime, int ipBanTime, int tradeLockTime, string defaultSanction)
        {
            this.Id = id;
            this.ParentId = parentId;
            this.Type = type;
            this.Caption = caption;
            this.MessageText = messageText;
            this.MuteTime = muteText;
            this.BanTime = banTime;
            this.IPBanTime = ipBanTime;
            this.TradeLockTime = tradeLockTime;
            this.DefaultSanction = defaultSanction;
        }
    }
}
