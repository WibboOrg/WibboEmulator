using Wibbo.Game.Rooms;
using Wibbo.Game.Users;
using Wibbo.Utilities;

namespace Wibbo.Communication.Packets.Outgoing.Moderation
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
                RoomData roomData = WibboEnvironment.GetGame().GetRoomManager().GenerateNullableRoomData(Visit.Value);

                WriteInteger(roomData.Id);
                WriteString(roomData.Name);
                WriteInteger(UnixTimestamp.FromUnixTimestamp(Visit.Key).Hour);
                WriteInteger(UnixTimestamp.FromUnixTimestamp(Visit.Key).Minute);
            }
        }
    }
}