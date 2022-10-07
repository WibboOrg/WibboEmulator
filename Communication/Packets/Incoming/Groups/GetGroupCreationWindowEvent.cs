namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class GetGroupCreationWindowEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.GetUser() == null)
        {
            return;
        }

        var validRooms = new List<RoomData>();
        foreach (var roomId in session.GetUser().UsersRooms)
        {
            var data = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(roomId);
            if (data == null)
            {
                continue;
            }

            if (data.Group == null)
            {
                validRooms.Add(data);
            }
        }

        session.SendPacket(new GroupCreationWindowComposer(validRooms));
    }
}
