using Wibbo.Communication.Packets.Outgoing.Navigator.New;

using Wibbo.Game.Clients;
using Wibbo.Game.Navigator;
using Wibbo.Utilities;

namespace Wibbo.Communication.Packets.Incoming.Structure
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