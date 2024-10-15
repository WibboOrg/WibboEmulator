namespace WibboEmulator.Communication.Packets.Incoming.Guide;
using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Games.GameClients;

internal sealed class GuideInviteToRoomEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var requester = GameClientManager.GetClientByUserID(Session.User.GuideOtherUserId);
        if (requester == null)
        {
            return;
        }

        var room = Session.User.Room;

        if (room == null)
        {
            requester.SendPacket(new OnGuideSessionInvitedToGuideRoomComposer(0, ""));
            Session.SendPacket(new OnGuideSessionInvitedToGuideRoomComposer(0, ""));
        }
        else
        {
            requester.SendPacket(new OnGuideSessionInvitedToGuideRoomComposer(room.Id, room.RoomData.Name));
            Session.SendPacket(new OnGuideSessionInvitedToGuideRoomComposer(room.Id, room.RoomData.Name));
        }
    }
}