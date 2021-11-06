using Butterfly.HabboHotel.Navigators;
using System.Collections.Generic;
using System.Linq;

namespace Butterfly.Communication.Packets.Outgoing.Navigator
{
    internal class NavigatorFlatCatsMessageComposer : ServerPacket
    {
        public NavigatorFlatCatsMessageComposer(ICollection<SearchResultList> Categories)
            : base(ServerPacketHeader.NAVIGATOR_EVENT_CATEGORIES)
        {
            WriteInteger(Categories.Count);
            foreach (SearchResultList category in Categories.ToList())
            {
                WriteInteger(category.Id);
                WriteString(category.PublicName);
                WriteBoolean(true);
            }

        }
    }
}
