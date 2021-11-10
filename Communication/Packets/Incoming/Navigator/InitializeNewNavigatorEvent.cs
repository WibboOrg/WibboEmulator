using Butterfly.Communication.Packets.Outgoing.Navigator.New;

using Butterfly.Game.GameClients;
using Butterfly.Game.Navigators;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class InitializeNewNavigatorEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            ICollection<TopLevelItem> TopLevelItems = ButterflyEnvironment.GetGame().GetNavigator().GetTopLevelItems();

            Session.SendPacket(new NavigatorMetaDataParserComposer(TopLevelItems));
            Session.SendPacket(new NavigatorLiftedRoomsComposer());
            Session.SendPacket(new NavigatorCollapsedCategoriesComposer());
            Session.SendPacket(new NavigatorPreferencesComposer());
        }
    }
}