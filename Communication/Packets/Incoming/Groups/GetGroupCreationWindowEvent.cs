using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class GetGroupCreationWindowEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetUser() == null)
            {
                return;
            }

            List<RoomData> ValidRooms = new List<RoomData>();
            foreach (int RoomId in Session.GetUser().UsersRooms)
            {
                RoomData Data = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);
                if (Data == null)
                    continue;

                if (Data.Group == null)
                {
                    ValidRooms.Add(Data);
                }
            }

            Session.SendPacket(new GroupCreationWindowComposer(ValidRooms));
        }
    }
}