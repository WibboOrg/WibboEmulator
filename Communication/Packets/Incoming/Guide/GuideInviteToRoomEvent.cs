namespace WibboEmulator.Communication.Packets.Incoming.Guide;
using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Games.GameClients;

internal class GuideInviteToRoomEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        var requester = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(session.GetUser().GuideOtherUserId);
        if (requester == null)
        {
            return;
        }

        var room = session.GetUser().CurrentRoom;

        if (room == null)
        {
            requester.SendPacket(new OnGuideSessionInvitedToGuideRoomComposer(0, ""));
            session.SendPacket(new OnGuideSessionInvitedToGuideRoomComposer(0, ""));
        }
        else
        {
            requester.SendPacket(new OnGuideSessionInvitedToGuideRoomComposer(room.Id, room.RoomData.Name));
            session.SendPacket(new OnGuideSessionInvitedToGuideRoomComposer(room.Id, room.RoomData.Name));
        }
    }
}