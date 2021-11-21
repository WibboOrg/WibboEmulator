using Butterfly.Game.GameClients;
using Butterfly.Utility;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class HabboSearchEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            string SearchPseudo = StringCharFilter.Escape(Packet.PopString());
            if (SearchPseudo.Length < 1 || SearchPseudo.Length > 100)
            {
                return;
            }

            Session.SendPacket(Session.GetHabbo().GetMessenger().PerformSearch(SearchPseudo));
        }
    }
}
