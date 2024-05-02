namespace WibboEmulator.Communication.Packets.Outgoing.Moderation;

using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Users;

internal sealed class ModeratorUserInfoComposer : ServerPacket
{
    public ModeratorUserInfoComposer(User user)
        : base(ServerPacketHeader.MODERATION_USER_INFO)
    {
        this.WriteInteger(user.Id);
        this.WriteString(user.Username);
        this.WriteString(user.Look);
        this.WriteInteger(Convert.ToInt32(Math.Ceiling((WibboEnvironment.GetUnixTimestamp() - Convert.ToDouble(user.AccountCreated)) / 60)));
        this.WriteInteger(Convert.ToInt32(Math.Ceiling((WibboEnvironment.GetUnixTimestamp() - Convert.ToDouble(user.LastOnline)) / 60)));
        this.WriteBoolean(GameClientManager.GetClientByUserID(user.Id) != null);
        this.WriteInteger(/*info != null ? Convert.ToInt32(info["cfhs"]) : */0);
        this.WriteInteger(/*info != null ? Convert.ToInt32(info["cfhs_abusive"]) : */0);
        this.WriteInteger(/*info != null ? Convert.ToInt32(info["cautions"]) : */0);
        this.WriteInteger(/*info != null ? Convert.ToInt32(info["bans"]) : */0);
        this.WriteInteger(/*info != null ? Convert.ToInt32(info["trading_locks_count"]) : */0);//Trading lock counts
        this.WriteString(/*info != null ? Convert.ToDouble(info["trading_locked"]) != 0 ? origin.ToString("dd/MM/yyyy HH:mm:ss") : "0" : */"0"); //Trading lock
        this.WriteString("");//Purchases
        this.WriteInteger(0);//Itendity information tool
        this.WriteInteger(0);//Id bans.
        this.WriteString("Unknown"); //User != null ? Convert.ToString(User["mail"]) : > private
        this.WriteString("");//user_classification
    }
}
