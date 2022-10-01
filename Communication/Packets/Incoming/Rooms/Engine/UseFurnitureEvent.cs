using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Quests;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class UseFurnitureEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room room))
                return;

            int Id = Packet.PopInt();

            Item RoomItem = room.GetRoomItemHandler().GetItem(Id);
            if (RoomItem == null)
            {
                return;
            }

            if (RoomItem.GetBaseItem().ItemName == "bw_lgchair")
            {
                WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.EXPLORE_FIND_ITEM, 1936);
            }
            else if (RoomItem.GetBaseItem().ItemName.Contains("bw_sboard"))
            {
                WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.EXPLORE_FIND_ITEM, 1969);
            }
            else if (RoomItem.GetBaseItem().ItemName.Contains("bw_van"))
            {
                WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.EXPLORE_FIND_ITEM, 1956);
            }
            else if (RoomItem.GetBaseItem().ItemName.Contains("party_floor"))
            {
                WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.EXPLORE_FIND_ITEM, 1369);
            }
            else if (RoomItem.GetBaseItem().ItemName.Contains("party_ball"))
            {
                WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.EXPLORE_FIND_ITEM, 1375);
            }
            else if (RoomItem.GetBaseItem().ItemName.Contains("jukebox"))
            {
                WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.EXPLORE_FIND_ITEM, 1019);
            }
            else if (RoomItem.GetBaseItem().ItemName.Contains("bb_gate"))
            {
                WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.EXPLORE_FIND_ITEM, 2050);
            }
            else if (RoomItem.GetBaseItem().ItemName == "bb_patch1")
            {
                WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.EXPLORE_FIND_ITEM, 2040);
            }
            else if (RoomItem.GetBaseItem().ItemName == "bb_rnd_tele")
            {
                WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.EXPLORE_FIND_ITEM, 2049);
            }
            else if (RoomItem.GetBaseItem().ItemName.Contains("es_gate_"))
            {
                WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.EXPLORE_FIND_ITEM, 2167);
            }
            else if (RoomItem.GetBaseItem().ItemName.Contains("es_score_"))
            {
                WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.EXPLORE_FIND_ITEM, 2172);
            }
            else if (RoomItem.GetBaseItem().ItemName.Contains("es_exit"))
            {
                WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.EXPLORE_FIND_ITEM, 2166);
            }
            else if (RoomItem.GetBaseItem().ItemName == "es_tagging")
            {
                WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.EXPLORE_FIND_ITEM, 2148);
            }

            bool UserHasRights = false;
            if (room.CheckRights(Session))
            {
                UserHasRights = true;
            }

            int Request = Packet.PopInt();

            RoomItem.Interactor.OnTrigger(Session, RoomItem, Request, UserHasRights, false);
            RoomItem.OnTrigger(room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id));
        }
    }
}
