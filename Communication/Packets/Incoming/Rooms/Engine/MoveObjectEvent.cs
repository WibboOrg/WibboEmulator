namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Engine;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;
using WibboEmulator.Games.Rooms;

internal sealed class MoveObjectEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (!RoomManager.TryGetRoom(Session.User.RoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(Session))
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
            Session.SendNotification(LanguageManager.TryGetValue("roomsell.error.7", Session.Language));
            return;
        }

        var newX = packet.PopInt();
        var newY = packet.PopInt();
        var newRot = packet.PopInt();
        _ = packet.PopInt();

        if (newX != roomItem.X || newY != roomItem.Y)
        {
            QuestManager.ProgressUserQuest(Session, QuestType.FurniMove, 0);
        }

        if (newRot != roomItem.Rotation)
        {
            QuestManager.ProgressUserQuest(Session, QuestType.FurniRotate, 0);
        }

        if (roomItem.Z >= 0.1)
        {
            QuestManager.ProgressUserQuest(Session, QuestType.FurniStack, 0);
        }

        if (!room.RoomItemHandling.SetFloorItem(Session, roomItem, newX, newY, newRot, false, false, true))
        {
            roomItem.UpdateState(false);
            return;
        }
    }
}
