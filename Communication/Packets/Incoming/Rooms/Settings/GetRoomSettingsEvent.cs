namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Settings;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class GetRoomSettingsEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var roomId = packet.PopInt();

        if (!RoomManager.TryGetRoom(roomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(Session, true) && !Session.User.HasPermission("settings_room"))
        {
            return;
        }

        Session.SendPacket(new RoomSettingsDataComposer(room.RoomData));
    }
}
