namespace WibboEmulator.Communication.Packets.Outgoing.Moderation;
using System.Data;

internal class ModeratorUserInfoComposer : ServerPacket
{
    public ModeratorUserInfoComposer(DataRow User, DataRow Info)
        : base(ServerPacketHeader.MODERATION_USER_INFO)
    {
        var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Info != null ? Convert.ToDouble(Info["trading_locked"]) : 0);

        this.WriteInteger(User != null ? Convert.ToInt32(User["id"]) : 0);
        this.WriteString(User != null ? Convert.ToString(User["username"]) : "Unknown");
        this.WriteString(User != null ? Convert.ToString(User["look"]) : "Unknown");
        this.WriteInteger(User != null ? Convert.ToInt32(Math.Ceiling((WibboEnvironment.GetUnixTimestamp() - Convert.ToDouble(User["account_created"])) / 60)) : 0);
        this.WriteInteger(User != null ? Convert.ToInt32(Math.Ceiling((WibboEnvironment.GetUnixTimestamp() - Convert.ToDouble(User["last_online"])) / 60)) : 0);
        this.WriteBoolean(User != null && WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(Convert.ToInt32(User["id"])) != null);
        this.WriteInteger(Info != null ? Convert.ToInt32(Info["cfhs"]) : 0);
        this.WriteInteger(Info != null ? Convert.ToInt32(Info["cfhs_abusive"]) : 0);
        this.WriteInteger(Info != null ? Convert.ToInt32(Info["cautions"]) : 0);
        this.WriteInteger(Info != null ? Convert.ToInt32(Info["bans"]) : 0);
        this.WriteInteger(Info != null ? Convert.ToInt32(Info["trading_locks_count"]) : 0);//Trading lock counts
        this.WriteString(Info != null ? Convert.ToDouble(Info["trading_locked"]) != 0 ? origin.ToString("dd/MM/yyyy HH:mm:ss") : "0" : "0"); //Trading lock
        this.WriteString("");//Purchases
        this.WriteInteger(0);//Itendity information tool
        this.WriteInteger(0);//Id bans.
        this.WriteString("Unknown"); //User != null ? Convert.ToString(User["mail"]) : > private
        this.WriteString("");//user_classification
    }
}
