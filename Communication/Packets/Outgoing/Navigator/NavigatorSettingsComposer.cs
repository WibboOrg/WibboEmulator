namespace Wibbo.Communication.Packets.Outgoing.Navigator
{
    internal class NavigatorSettingsComposer : ServerPacket
    {
        public NavigatorSettingsComposer(int windowX, int windowY, int windowWidth, int windowHeight, bool leftPanelHidden, int resultsMode)
            : base(ServerPacketHeader.NAVIGATOR_SETTINGS)
        {
            WriteInteger(windowX);
            WriteInteger(windowY);
            WriteInteger(windowWidth);
            WriteInteger(windowHeight);
            WriteBoolean(leftPanelHidden);
            WriteInteger(resultsMode);
        }
    }
}
