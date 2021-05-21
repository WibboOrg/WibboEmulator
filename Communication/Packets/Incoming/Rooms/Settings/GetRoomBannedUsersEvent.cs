using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetRoomBannedUsersEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            Room Instance = Session.GetHabbo().CurrentRoom;
            if (Instance == null || !Instance.CheckRights(Session, true))
            {
                return;
            }

            if (Instance.getBans().Count > 0)
            {
                Session.SendPacket(new GetRoomBannedUsersComposer(Instance));
            }
        }
    }
}