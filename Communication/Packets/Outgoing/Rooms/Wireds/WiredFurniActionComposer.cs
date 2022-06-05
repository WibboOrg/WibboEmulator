namespace Butterfly.Communication.Packets.Outgoing.Rooms.Wireds
{
    internal class WiredFurniActionComposer : ServerPacket
    {
        public WiredFurniActionComposer(bool stuffTypeSelectionEnabled, int furniLimit, List<int> stuffIds, int stuffTypeId, int id, string stringParam, 
            List<int> intParams, int stuffTypeSelectionCode, int type, int delayInPulses, List<int> conflictingTriggers)
            : base(ServerPacketHeader.WIRED_ACTION)
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
            WriteInteger(delayInPulses);

            WriteInteger(conflictingTriggers.Count);
            foreach (int conflictingTrigger in conflictingTriggers)
                WriteInteger(conflictingTrigger);
        }
    }
}
