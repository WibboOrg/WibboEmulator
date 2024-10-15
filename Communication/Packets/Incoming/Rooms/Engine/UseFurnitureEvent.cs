namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;
using WibboEmulator.Games.Rooms;

internal sealed class UseFurnitureEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (!RoomManager.TryGetRoom(Session.User.RoomId, out var room))
        {
            return;
        }

        var id = packet.PopInt();

        var roomItem = room.RoomItemHandling.GetItem(id);
        if (roomItem == null)
        {
            return;
        }

        if (roomItem.ItemData.ItemName == "bw_lgchair")
        {
            QuestManager.ProgressUserQuest(Session, QuestType.ExploreFindItem, 1936);
        }
        else if (roomItem.ItemData.ItemName.Contains("bw_sboard"))
        {
            QuestManager.ProgressUserQuest(Session, QuestType.ExploreFindItem, 1969);
        }
        else if (roomItem.ItemData.ItemName.Contains("bw_van"))
        {
            QuestManager.ProgressUserQuest(Session, QuestType.ExploreFindItem, 1956);
        }
        else if (roomItem.ItemData.ItemName.Contains("party_floor"))
        {
            QuestManager.ProgressUserQuest(Session, QuestType.ExploreFindItem, 1369);
        }
        else if (roomItem.ItemData.ItemName.Contains("party_ball"))
        {
            QuestManager.ProgressUserQuest(Session, QuestType.ExploreFindItem, 1375);
        }
        else if (roomItem.ItemData.ItemName.Contains("jukebox"))
        {
            QuestManager.ProgressUserQuest(Session, QuestType.ExploreFindItem, 1019);
        }
        else if (roomItem.ItemData.ItemName.Contains("bb_gate"))
        {
            QuestManager.ProgressUserQuest(Session, QuestType.ExploreFindItem, 2050);
        }
        else if (roomItem.ItemData.ItemName == "bb_patch1")
        {
            QuestManager.ProgressUserQuest(Session, QuestType.ExploreFindItem, 2040);
        }
        else if (roomItem.ItemData.ItemName == "bb_rnd_tele")
        {
            QuestManager.ProgressUserQuest(Session, QuestType.ExploreFindItem, 2049);
        }
        else if (roomItem.ItemData.ItemName.Contains("es_gate_"))
        {
            QuestManager.ProgressUserQuest(Session, QuestType.ExploreFindItem, 2167);
        }
        else if (roomItem.ItemData.ItemName.Contains("es_score_"))
        {
            QuestManager.ProgressUserQuest(Session, QuestType.ExploreFindItem, 2172);
        }
        else if (roomItem.ItemData.ItemName.Contains("es_exit"))
        {
            QuestManager.ProgressUserQuest(Session, QuestType.ExploreFindItem, 2166);
        }
        else if (roomItem.ItemData.ItemName == "es_tagging")
        {
            QuestManager.ProgressUserQuest(Session, QuestType.ExploreFindItem, 2148);
        }

        var userHasRights = false;
        if (room.CheckRights(Session))
        {
            userHasRights = true;
        }

        var request = packet.PopInt();

        roomItem.Interactor.OnTrigger(Session, roomItem, request, userHasRights, false);
        roomItem.OnTrigger(room.RoomUserManager.GetRoomUserByUserId(Session.User.Id));
    }
}
