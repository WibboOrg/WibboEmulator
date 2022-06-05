using Wibbo.Communication.Packets.Outgoing.Groups;
using Wibbo.Game.Clients;
using Wibbo.Game.Groups;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class GetGroupInfoEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();
            bool NewWindow = Packet.PopBoolean();

            if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
            {
                return;
            }

            Session.SendPacket(new GroupInfoComposer(Group, Session, NewWindow));
        }
    }
}
