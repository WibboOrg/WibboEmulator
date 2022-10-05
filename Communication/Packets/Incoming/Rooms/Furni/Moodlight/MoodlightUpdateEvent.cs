namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal class MoodlightUpdateEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
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

        var Preset = Packet.PopInt();
        var num = Packet.PopInt();
        var Color = Packet.PopString();
        var Intensity = Packet.PopInt();

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
