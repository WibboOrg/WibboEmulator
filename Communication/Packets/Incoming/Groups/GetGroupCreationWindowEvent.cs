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

        var ValidRooms = new List<RoomData>();
        foreach (var RoomId in session.GetUser().UsersRooms)
        {
            var Data = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);
            if (Data == null)
            {
                continue;
            }

            if (Data.Group == null)
            {
                ValidRooms.Add(Data);
            }
        }

        session.SendPacket(new GroupCreationWindowComposer(ValidRooms));
    }
}