namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni.Moodlight;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal class MoodlightUpdateEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session, true) || room.MoodlightData == null)
        {
            return;
        }

        var roomItem = room.GetRoomItemHandler().GetItem(room.MoodlightData.ItemId);
        if (roomItem == null || roomItem.GetBaseItem().InteractionType != InteractionType.MOODLIGHT)
        {
            return;
        }

        var Preset = packet.PopInt();
        var num = packet.PopInt();
        var Color = packet.PopString();
        var Intensity = packet.PopInt();

        var BgOnly = false;

        if (num >= 2)
        {
            BgOnly = true;
        }

        room.MoodlightData.Enabled = true;
        room.MoodlightData.CurrentPreset = Preset;
        room.MoodlightData.UpdatePreset(Preset, Color, Intensity, BgOnly);
        roomItem.ExtraData = room.MoodlightData.GenerateExtraData();
        roomItem.UpdateState();
    }
}
