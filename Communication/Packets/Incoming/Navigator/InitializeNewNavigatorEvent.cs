using Butterfly.Communication.Packets.Outgoing.Navigator.New;

using Butterfly.Game.Clients;
using Butterfly.Game.Navigator;
using Butterfly.Utilities;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class InitializeNewNavigatorEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            ICollection<TopLevelItem> TopLevelItems = ButterflyEnvironment.GetGame().GetNavigator().GetTopLevelItems();

            ServerPacketList packetList = new ServerPacketList();
            packetList.Add(new NavigatorMetaDataParserComposer(TopLevelItems));
            packetList.Add(new NavigatorLiftedRoomsComposer());
            packetList.Add(new NavigatorCollapsedCategoriesComposer());
            packetList.Add(new NavigatorPreferencesComposer());

            Session.SendPacket(packetList);
        }
    }
}