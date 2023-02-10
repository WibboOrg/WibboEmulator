namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Wireds;

internal sealed class WiredFurniConditionComposer : ServerPacket
{
    public WiredFurniConditionComposer(bool stuffTypeSelectionEnabled, int furniLimit, List<int> stuffIds, int stuffTypeId, int id, string stringParam,
        List<int> intParams, int stuffTypeSelectionCode, int type)
        : base(ServerPacketHeader.WIRED_CONDITION)
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
    }
}
