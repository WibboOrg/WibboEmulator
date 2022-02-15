using Butterfly.Game.Rooms;
using Butterfly.Game.Users;
using Butterfly.Utilities;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorUserRoomVisitsComposer : ServerPacket
    {
        public ModeratorUserRoomVisitsComposer(User Data, Dictionary<double, int> Visits)
            : base(ServerPacketHeader.MODTOOL_VISITED_ROOMS_USER)
        {
            WriteInteger(Data.Id);
            WriteString(Data.Username);
            WriteInteger(Visits.Count);

            foreach (KeyValuePair<double, int> Visit in Visits)
            {
                RoomData roomData = ButterflyEnvironment.GetGame().GetRoomManager().GenerateNullableRoomData(Visit.Value);

                WriteInteger(roomData.Id);
                WriteString(roomData.Name);
                WriteInteger(UnixTimestamp.FromUnixTimestamp(Visit.Key).Hour);
                WriteInteger(UnixTimestamp.FromUnixTimestamp(Visit.Key).Minute);
            }
        }
    }
}