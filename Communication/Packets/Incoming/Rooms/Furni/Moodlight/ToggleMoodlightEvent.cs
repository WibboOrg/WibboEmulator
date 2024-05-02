namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni.Moodlight;

using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

internal sealed class ToggleMoodlightEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!RoomManager.TryGetRoom(session.User.RoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session, true) || room.MoodlightData == null)
        {
            return;
        }

        var roomItem = room.RoomItemHandling.GetItem(room.MoodlightData.ItemId);
        if (roomItem == null || roomItem.ItemData.InteractionType != InteractionType.MOODLIGHT)
        {
            return;
        }

        if (room.MoodlightData.Enabled)
        {
            room.MoodlightData.Disable();
        }
        else
        {
            room.MoodlightData.Enable();
        }

        using var dbClient = DatabaseManager.Connection;
        ItemMoodlightDao.UpdateEnabled(dbClient, room.MoodlightData.ItemId, room.MoodlightData.Enabled);

        roomItem.ExtraData = room.MoodlightData.GenerateExtraData();
        roomItem.UpdateState();
    }
}
