namespace WibboEmulator.Communication.Packets.Outgoing.Navigator;

internal sealed class NavigatorSettingsComposer : ServerPacket
{
    public NavigatorSettingsComposer(int windowX, int windowY, int windowWidth, int windowHeight, bool leftPanelHidden, int resultsMode)
        : base(ServerPacketHeader.NAVIGATOR_SETTINGS)
    {
        this.WriteInteger(windowX);
        this.WriteInteger(windowY);
        this.WriteInteger(windowWidth);
        this.WriteInteger(windowHeight);
        this.WriteBoolean(leftPanelHidden);
        this.WriteInteger(resultsMode);
    }
}
