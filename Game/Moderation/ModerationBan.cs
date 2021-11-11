namespace Butterfly.Game.Moderation
{
    public class ModerationBan
    {
        public ModerationBanType Type { get; set; }

        public string Value { get; set; }

        public string Reason { get; set; }

        public double Expire { get; set; }

        public ModerationBan(ModerationBanType type, string value, string reason, double expire)
        {

            Type = type;
            Value = value;
            Reason = reason;
            Expire = expire;
        }

        public bool Expired
        {
            get
            {
                if (ButterflyEnvironment.GetUnixTimestamp() >= Expire)
                    return true;
                return false;
            }
        }
    }
}