namespace WibboEmulator.Communication.Packets.Incoming.Guide;
using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Games.GameClients;

internal sealed class VisitRoomGuidesEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var requester = GameClientManager.GetClientByUserID(Session.User.GuideOtherUserId);
        if (requester == null)
        {
            return;
        }

        Session.SendPacket(new OnGuideSessionRequesterRoomComposer(requester.User.RoomId));
    }
}
