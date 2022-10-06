namespace WibboEmulator.Communication.Packets.Incoming.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Navigator.New;

using WibboEmulator.Games.GameClients;
using WibboEmulator.Utilities;

internal class InitializeNewNavigatorEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var TopLevelItems = WibboEnvironment.GetGame().GetNavigator().GetTopLevelItems();

        var packetList = new ServerPacketList();
        packetList.Add(new NavigatorMetaDataParserComposer(TopLevelItems));
        packetList.Add(new NavigatorLiftedRoomsComposer());
        packetList.Add(new NavigatorCollapsedCategoriesComposer());
        packetList.Add(new NavigatorPreferencesComposer());

        session.SendPacket(packetList);
    }
}