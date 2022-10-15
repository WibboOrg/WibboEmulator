namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Engine;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;

internal class MoveObjectEvent : IPacketEvent
{
    public double Delay => 200;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.User.CurrentRoomId, out var room))
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
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.error.7", session.Langue));
            return;
        }

        var newX = packet.PopInt();
        var newY = packet.PopInt();
        var newRot = packet.PopInt();
        _ = packet.PopInt();

        if (newX != roomItem.X || newY != roomItem.Y)
        {
            WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.FurniMove, 0);
        }

        if (newRot != roomItem.Rotation)
        {
            WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.FurniRotate, 0);
        }

        if (roomItem.Z >= 0.1)
        {
            WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.FurniStack, 0);
        }

        if (!room.RoomItemHandling.SetFloorItem(session, roomItem, newX, newY, newRot, false, false, true))
        {
            room.SendPacket(new ObjectUpdateComposer(roomItem, room.RoomData.OwnerId));
            return;
        }
    }
}
