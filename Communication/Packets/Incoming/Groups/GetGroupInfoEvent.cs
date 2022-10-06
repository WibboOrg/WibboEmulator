namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Games.GameClients;

internal class GetGroupInfoEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var GroupId = packet.PopInt();
        var NewWindow = packet.PopBoolean();

        if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out var Group))
        {
            return;
        }

        session.SendPacket(new GroupInfoComposer(Group, session, NewWindow));
    }
}
