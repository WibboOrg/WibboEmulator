using Wibbo.Game.Clients;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class GetAchievementsEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            WibboEnvironment.GetGame().GetAchievementManager().GetList(Session);
        }
    }
}