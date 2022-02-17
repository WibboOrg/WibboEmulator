using Butterfly.Game.Clients;namespace Butterfly.Communication.Packets.Incoming.Structure{    internal class MoveMonsterPlanteEvent : IPacketEvent    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)        {


        }    }}