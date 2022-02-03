using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorRoomInfoComposer : ServerPacket
    {
        public ModeratorRoomInfoComposer(RoomData data, bool ownerInRoom)
            : base(ServerPacketHeader.MODTOOL_ROOM_INFO)
        {
            WriteInteger(data.Id);
            WriteInteger(data.UsersNow);

            WriteBoolean(ownerInRoom);

            WriteInteger(data.OwnerId);
            WriteString(data.OwnerName);
            WriteBoolean(true);

            WriteString(data.Name);
            WriteString(data.Description);
            WriteInteger(data.TagCount);
            foreach (string s in data.Tags)
            {
                WriteString(s);
            }
        }
    }
}
