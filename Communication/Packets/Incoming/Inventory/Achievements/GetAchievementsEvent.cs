using Butterfly.Game.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetAchievementsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            ButterflyEnvironment.GetGame().GetAchievementManager().GetList(Session);
        }
    }
}