using Butterfly.Game.Clients;
using System;
using System.Data;

namespace Butterfly.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorUserInfoComposer : ServerPacket
    {
        public ModeratorUserInfoComposer(DataRow row, Client client)
            : base(ServerPacketHeader.MODERATION_USER_INFO)
        {
            WriteInteger((row == null) ? client.GetHabbo().Id : Convert.ToInt32(row["id"]));
            WriteString((row == null) ? client.GetHabbo().Username : (string)row["username"]);
            WriteString("Unknown");

            WriteInteger(0);
            WriteInteger(0);

            WriteBoolean(client != null);

            WriteInteger(0);
            WriteInteger(0);
            WriteInteger(0);
            WriteInteger(0);

            WriteInteger(0); // trading_lock_count_txt

            WriteString("");
            WriteString("");
            WriteInteger(0);
            WriteInteger(0);
            WriteString("Unknown");
            WriteString(""); // ???
        }
    }
}
