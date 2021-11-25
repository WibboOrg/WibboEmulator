using Butterfly.Game.Moderation;

namespace Butterfly.HabboHotel.Moderation
{
    public static class BanTypeUtility
    {
        public static ModerationBanType GetModerationBanType(string type)
        {
            switch (type)
            {
                default:
                case "user":
                    return ModerationBanType.USER;
                case "ip":
                    return ModerationBanType.IP;
                case "machine":
                    return ModerationBanType.MACHINE;
            }
        }

        public static string FromModerationBanType(ModerationBanType type)
        {
            switch (type)
            {
                default:
                case ModerationBanType.USER:
                    return "user";
                case ModerationBanType.IP:
                    return "ip";
                case ModerationBanType.MACHINE:
                    return "machine";
            }
        }
    }
}
