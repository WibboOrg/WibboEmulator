using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Groups;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
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
