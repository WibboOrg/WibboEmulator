namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Settings;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;
using WibboEmulator.Games.GameClients;

internal class GetRoomSettingsEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var roomId = packet.PopInt();

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(roomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session, true) && !session.User.HasPermission("perm_settings_room"))
        {
            return;
        }

        session.SendPacket(new RoomSettingsDataComposer(room.RoomData));
    }
}