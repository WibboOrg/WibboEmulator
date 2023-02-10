namespace WibboEmulator.Communication.Packets.Outgoing.Moderation;
using System.Data;

internal sealed class ModeratorUserInfoComposer : ServerPacket
{
    public ModeratorUserInfoComposer(DataRow user)
        : base(ServerPacketHeader.MODERATION_USER_INFO)
    {
        //var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(info != null ? Convert.ToDouble(info["trading_locked"]) : 0);

        this.WriteInteger(user != null ? Convert.ToInt32(user["id"]) : 0);
        this.WriteString(user != null ? Convert.ToString(user["username"]) : "Unknown");
        this.WriteString(user != null ? Convert.ToString(user["look"]) : "Unknown");
        this.WriteInteger(user != null ? Convert.ToInt32(Math.Ceiling((WibboEnvironment.GetUnixTimestamp() - Convert.ToDouble(user["account_created"])) / 60)) : 0);
        this.WriteInteger(user != null ? Convert.ToInt32(Math.Ceiling((WibboEnvironment.GetUnixTimestamp() - Convert.ToDouble(user["last_online"])) / 60)) : 0);
        this.WriteBoolean(user != null && WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(Convert.ToInt32(user["id"])) != null);
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
