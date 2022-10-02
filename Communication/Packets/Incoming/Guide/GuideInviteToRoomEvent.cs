using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Guide
{
    internal class GuideInviteToRoomEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            GameClient requester = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(Session.GetUser().GuideOtherUserId);
            if (requester == null)
            {
                return;
            }

            Room room = Session.GetUser().CurrentRoom;

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
}