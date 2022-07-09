namespace Wibbo.Communication.Packets.Outgoing.Rooms.Wireds
{
    internal class WiredFurniTriggerComposer : ServerPacket
    {
        public WiredFurniTriggerComposer(bool stuffTypeSelectionEnabled, int furniLimit, List<int> stuffIds, int stuffTypeId, int id, string stringParam, 
            List<int> intParams, int stuffTypeSelectionCode, int type, List<int> conflictingActions)
            : base(ServerPacketHeader.WIRED_TRIGGER)
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

            WriteInteger(conflictingActions.Count);
            foreach (int conflictingAction in conflictingActions)
                WriteInteger(conflictingAction);
        }
    }
}
