namespace WibboEmulator.Communication.Packets.Outgoing.Handshake;
using WibboEmulator.Games.Users;

internal class UserPerksComposer : ServerPacket
{
    public UserPerksComposer(User user)
        : base(ServerPacketHeader.USER_PERKS)
    {
        this.WriteInteger(17); // Count
        this.WriteString("USE_GUIDE_TOOL");
        this.WriteString("");
        this.WriteBoolean(user.HasPermission("helptool"));

        this.WriteString("GIVE_GUIDE_TOURS");
        this.WriteString("requirement.unfulfilled.helper_le");
        this.WriteBoolean(false);

        this.WriteString("JUDGE_CHAT_REVIEWS");
        this.WriteString(""); // ??
        this.WriteBoolean(true);

        this.WriteString("VOTE_IN_COMPETITIONS");
        this.WriteString(""); // ??
        this.WriteBoolean(true);

        this.WriteString("CALL_ON_HELPERS");
        this.WriteString(""); // ??
        this.WriteBoolean(true);

        this.WriteString("CITIZEN");
        this.WriteString(""); // ??
        this.WriteBoolean(true);

        this.WriteString("TRADE");
        this.WriteString(""); // ??
        this.WriteBoolean(true);

        this.WriteString("HEIGHTMAP_EDITOR_BETA");
        this.WriteString(""); // ??
        this.WriteBoolean(false);

        this.WriteString("EXPERIMENTAL_CHAT_BETA");
        this.WriteString("requirement.unfulfilled.helper_level_2");
        this.WriteBoolean(true);

        this.WriteString("EXPERIMENTAL_TOOLBAR");
        this.WriteString(""); // ??
        this.WriteBoolean(true);

        this.WriteString("BUILDER_AT_WORK");
        this.WriteString(""); // ??
        this.WriteBoolean(true);

        this.WriteString("NAVIGATOR_PHASE_ONE_2014");
        this.WriteString(""); // ??
        this.WriteBoolean(false);

        this.WriteString("CAMERA");
        this.WriteString(""); // ??
        this.WriteBoolean(true);

        this.WriteString("NAVIGATOR_PHASE_TWO_2014");
        this.WriteString(""); // ??
        this.WriteBoolean(true);

        this.WriteString("MOUSE_ZOOM");
        this.WriteString(""); // ??
        this.WriteBoolean(true);

        this.WriteString("NAVIGATOR_ROOM_THUMBNAIL_CAMERA");
        this.WriteString(""); // ??
        this.WriteBoolean(true);

        this.WriteString("HABBO_CLUB_OFFER_BETA");
        this.WriteString(""); // ??
        this.WriteBoolean(true);
    }
}
