using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorRoomInfoComposer : ServerPacket
    {
        public ModeratorRoomInfoComposer(RoomData Data, bool OwnerInRoom)
            : base(ServerPacketHeader.MODTOOL_ROOM_INFO)
        {
            WriteInteger(Data.Id);
            WriteInteger(Data.UsersNow);
            WriteBoolean(OwnerInRoom); // owner in room
            WriteInteger(Data.OwnerId);
            WriteString(Data.OwnerName);
            WriteBoolean(Data != null);
            WriteString(Data.Name);
            WriteString(Data.Description);

            WriteInteger(Data.Tags.Count);
            foreach (string Tag in Data.Tags)
            {
                WriteString(Tag);
            }

            WriteBoolean(false);
        }
    }
}
