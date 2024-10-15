namespace WibboEmulator.Communication.Packets.Incoming.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Navigator.New;

using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Navigators;
using WibboEmulator.Utilities;

internal sealed class InitializeNewNavigatorEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var topLevelItems = NavigatorManager.TopLevelItems;

        var packetList = new ServerPacketList();
        packetList.Add(new NavigatorMetaDataParserComposer(topLevelItems));
        packetList.Add(new NavigatorLiftedRoomsComposer());
        packetList.Add(new NavigatorCollapsedCategoriesComposer());

        Session.SendPacket(packetList);
    }
}
