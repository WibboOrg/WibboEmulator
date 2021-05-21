using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetCreditsInfoEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendPacket(new CreditBalanceComposer(Session.GetHabbo().Credits));
            Session.SendPacket(new ActivityPointsComposer(Session.GetHabbo().Duckets, Session.GetHabbo().WibboPoints));
        }
    }
}
