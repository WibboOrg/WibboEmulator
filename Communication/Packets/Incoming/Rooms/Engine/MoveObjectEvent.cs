namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Engine;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;
using WibboEmulator.Games.Rooms;

internal sealed class MoveObjectEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!RoomManager.TryGetRoom(session.User.RoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session))
        {
            return;
        }

        var roomItem = room.RoomItemHandling.GetItem(packet.PopInt());
        if (roomItem == null)
        {
            return;
        }

        if (room.RoomData.SellPrice > 0)
        {
            session.SendNotification(LanguageManager.TryGetValue("roomsell.error.7", session.Language));
            return;
        }

        var newX = packet.PopInt();
        var newY = packet.PopInt();
        var newRot = packet.PopInt();
        _ = packet.PopInt();

        if (newX != roomItem.X || newY != roomItem.Y)
        {
            QuestManager.ProgressUserQuest(session, QuestType.FurniMove, 0);
        }

        if (newRot != roomItem.Rotation)
        {
            QuestManager.ProgressUserQuest(session, QuestType.FurniRotate, 0);
        }

        if (roomItem.Z >= 0.1)
        {
            QuestManager.ProgressUserQuest(session, QuestType.FurniStack, 0);
        }

        if (!room.RoomItemHandling.SetFloorItem(session, roomItem, newX, newY, newRot, false, false, true))
        {
            roomItem.UpdateState(false);
            return;
        }
    }
}
