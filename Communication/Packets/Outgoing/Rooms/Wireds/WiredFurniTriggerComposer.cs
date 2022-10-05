namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Wireds;

internal class WiredFurniTriggerComposer : ServerPacket
{
    public WiredFurniTriggerComposer(bool stuffTypeSelectionEnabled, int furniLimit, List<int> stuffIds, int stuffTypeId, int id, string stringParam,
        List<int> intParams, int stuffTypeSelectionCode, int type, List<int> conflictingActions)
        : base(ServerPacketHeader.WIRED_TRIGGER)
    {
        this.WriteBoolean(stuffTypeSelectionEnabled);
        this.WriteInteger(furniLimit);

        this.WriteInteger(stuffIds.Count);
        foreach (var stuffId in stuffIds)
        {
            this.WriteInteger(stuffId);
        }

        this.WriteInteger(stuffTypeId);
        this.WriteInteger(id);
        this.WriteString(stringParam);

        this.WriteInteger(intParams.Count);
        foreach (var intParam in intParams)
        {
            this.WriteInteger(intParam);
        }

        this.WriteInteger(stuffTypeSelectionCode);
        this.WriteInteger(type);

        this.WriteInteger(conflictingActions.Count);
        foreach (var conflictingAction in conflictingActions)
        {
            this.WriteInteger(conflictingAction);
        }
    }
}
