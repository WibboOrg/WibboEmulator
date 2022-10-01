using WibboEmulator.Communication.Packets.Outgoing.Navigator.New;

using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Navigator;
using WibboEmulator.Utilities;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class InitializeNewNavigatorEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            ICollection<TopLevelItem> TopLevelItems = WibboEnvironment.GetGame().GetNavigator().GetTopLevelItems();

            ServerPacketList packetList = new ServerPacketList();
            packetList.Add(new NavigatorMetaDataParserComposer(TopLevelItems));
            packetList.Add(new NavigatorLiftedRoomsComposer());
            packetList.Add(new NavigatorCollapsedCategoriesComposer());
            packetList.Add(new NavigatorPreferencesComposer());

            Session.SendPacket(packetList);
        }
    }
}