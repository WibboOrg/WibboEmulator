namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni.Moodlight;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Furni.Moodlight;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class GetMoodlightConfigEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (!RoomManager.TryGetRoom(Session.User.RoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(Session, true))
        {
            return;
        }

        if (room.MoodlightData == null || room.MoodlightData.Presets == null)
        {
            return;
        }

        Session.SendPacket(new MoodlightConfigComposer(room.MoodlightData));
    }
}
