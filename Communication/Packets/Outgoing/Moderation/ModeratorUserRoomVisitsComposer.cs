using Butterfly.Game.Rooms;
using Butterfly.Game.Users;
using Butterfly.Utility;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorUserRoomVisitsComposer : ServerPacket
    {
        public ModeratorUserRoomVisitsComposer(User Data, Dictionary<double, RoomData> Visits)
            : base(ServerPacketHeader.MODTOOL_VISITED_ROOMS_USER)
        {
            WriteInteger(Data.Id);
            WriteString(Data.Username);
            WriteInteger(Visits.Count);

            foreach (KeyValuePair<double, RoomData> Visit in Visits)
            {
                WriteInteger(Visit.Value.Id);
                WriteString(Visit.Value.Name);
                WriteInteger(UnixTimestamp.FromUnixTimestamp(Visit.Key).Hour);
                WriteInteger(UnixTimestamp.FromUnixTimestamp(Visit.Key).Minute);
            }
        }
    }
}