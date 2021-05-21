using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Navigators;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetUserFlatCatsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            ICollection<SearchResultList> Categories = ButterflyEnvironment.GetGame().GetNavigator().GetFlatCategories();

            Session.SendPacket(new UserFlatCatsComposer(Categories, Session.GetHabbo().Rank));
        }
    }
}