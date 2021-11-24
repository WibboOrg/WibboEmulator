using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Outgoing.Rooms.Wireds
{
    internal class WiredFurniConditionComposer : ServerPacket
    {
        public WiredFurniConditionComposer(bool stuffTypeSelectionEnabled, int furniLimit, List<int> stuffIds, int stuffTypeId, int id, string stringParam, 
            List<int> intParams, int stuffTypeSelectionCode, int type)
            : base(ServerPacketHeader.WIRED_CONDITION)
        {
            WriteBoolean(stuffTypeSelectionEnabled);
            WriteInteger(furniLimit);

            WriteInteger(stuffIds.Count);
            foreach(int stuffId in stuffIds)
                WriteInteger(stuffId);

            WriteInteger(stuffTypeId);
            WriteInteger(id);
            WriteString(stringParam);

            WriteInteger(intParams.Count);
            foreach (int intParam in intParams)
                WriteInteger(intParam);

            WriteInteger(stuffTypeSelectionCode);
            WriteInteger(type);
        }
    }
}
