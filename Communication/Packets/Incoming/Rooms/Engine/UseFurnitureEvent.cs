namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;

internal class UseFurnitureEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.User.CurrentRoomId, out var room))
        {
            return;
        }

        var id = packet.PopInt();

        var roomItem = room.RoomItemHandling.GetItem(id);
        if (roomItem == null)
        {
            return;
        }

        if (roomItem.GetBaseItem().ItemName == "bw_lgchair")
        {
            WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.ExploreFindItem, 1936);
        }
        else if (roomItem.GetBaseItem().ItemName.Contains("bw_sboard"))
        {
            WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.ExploreFindItem, 1969);
        }
        else if (roomItem.GetBaseItem().ItemName.Contains("bw_van"))
        {
            WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.ExploreFindItem, 1956);
        }
        else if (roomItem.GetBaseItem().ItemName.Contains("party_floor"))
        {
            WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.ExploreFindItem, 1369);
        }
        else if (roomItem.GetBaseItem().ItemName.Contains("party_ball"))
        {
            WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.ExploreFindItem, 1375);
        }
        else if (roomItem.GetBaseItem().ItemName.Contains("jukebox"))
        {
            WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.ExploreFindItem, 1019);
        }
        else if (roomItem.GetBaseItem().ItemName.Contains("bb_gate"))
        {
            WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.ExploreFindItem, 2050);
        }
        else if (roomItem.GetBaseItem().ItemName == "bb_patch1")
        {
            WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.ExploreFindItem, 2040);
        }
        else if (roomItem.GetBaseItem().ItemName == "bb_rnd_tele")
        {
            WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.ExploreFindItem, 2049);
        }
        else if (roomItem.GetBaseItem().ItemName.Contains("es_gate_"))
        {
            WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.ExploreFindItem, 2167);
        }
        else if (roomItem.GetBaseItem().ItemName.Contains("es_score_"))
        {
            WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.ExploreFindItem, 2172);
        }
        else if (roomItem.GetBaseItem().ItemName.Contains("es_exit"))
        {
            WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.ExploreFindItem, 2166);
        }
        else if (roomItem.GetBaseItem().ItemName == "es_tagging")
        {
            WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.ExploreFindItem, 2148);
        }

        var userHasRights = false;
        if (room.CheckRights(session))
        {
            userHasRights = true;
        }

        var request = packet.PopInt();

        roomItem.Interactor.OnTrigger(session, roomItem, request, userHasRights, false);
        roomItem.OnTrigger(room.RoomUserManager.GetRoomUserByUserId(session.User.Id));
    }
}
