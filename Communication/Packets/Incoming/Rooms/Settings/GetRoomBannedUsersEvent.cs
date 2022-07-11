using WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class GetRoomBannedUsersEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!Session.GetUser().InRoom)
            {
                return;
            }

            Room Instance = Session.GetUser().CurrentRoom;
            if (Instance == null || !Instance.CheckRights(Session, true))
            {
                return;
            }

            if (Instance.GetBans().Count > 0)
            {
                Session.SendPacket(new GetRoomBannedUsersComposer(Instance));
            }
        }
    }
}