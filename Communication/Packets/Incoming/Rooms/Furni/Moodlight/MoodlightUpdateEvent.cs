namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni.Moodlight;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

internal sealed class MoodlightUpdateEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (!RoomManager.TryGetRoom(Session.User.RoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(Session, true) || room.MoodlightData == null)
        {
            return;
        }

        var roomItem = room.RoomItemHandling.GetItem(room.MoodlightData.ItemId);
        if (roomItem == null || roomItem.ItemData.InteractionType != InteractionType.MOODLIGHT)
        {
            return;
        }

        var preset = packet.PopInt();
        var num = packet.PopInt();
        var color = packet.PopString();
        var intensity = packet.PopInt();

        var bgOnly = false;

        if (num >= 2)
        {
            bgOnly = true;
        }

        room.MoodlightData.Enabled = true;
        room.MoodlightData.CurrentPreset = preset;
        room.MoodlightData.UpdatePreset(preset, color, intensity, bgOnly);
        roomItem.ExtraData = room.MoodlightData.GenerateExtraData();
        roomItem.UpdateState();
    }
}
