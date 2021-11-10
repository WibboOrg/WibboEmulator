using Butterfly.Game.GameClients;
using Butterfly.Game.Items;
using Butterfly.Game.Quests;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class UseFurnitureEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            int Id = Packet.PopInt();

            Item RoomItem = room.GetRoomItemHandler().GetItem(Id);
            if (RoomItem == null)
            {
                return;
            }

            if (RoomItem.GetBaseItem().ItemName == "bw_lgchair")
            {
                ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.EXPLORE_FIND_ITEM, 1936);
            }
            else if (RoomItem.GetBaseItem().ItemName.Contains("bw_sboard"))
            {
                ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.EXPLORE_FIND_ITEM, 1969);
            }
            else if (RoomItem.GetBaseItem().ItemName.Contains("bw_van"))
            {
                ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.EXPLORE_FIND_ITEM, 1956);
            }
            else if (RoomItem.GetBaseItem().ItemName.Contains("party_floor"))
            {
                ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.EXPLORE_FIND_ITEM, 1369);
            }
            else if (RoomItem.GetBaseItem().ItemName.Contains("party_ball"))
            {
                ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.EXPLORE_FIND_ITEM, 1375);
            }
            else if (RoomItem.GetBaseItem().ItemName.Contains("jukebox"))
            {
                ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.EXPLORE_FIND_ITEM, 1019);
            }
            else if (RoomItem.GetBaseItem().ItemName.Contains("bb_gate"))
            {
                ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.EXPLORE_FIND_ITEM, 2050);
            }
            else if (RoomItem.GetBaseItem().ItemName == "bb_patch1")
            {
                ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.EXPLORE_FIND_ITEM, 2040);
            }
            else if (RoomItem.GetBaseItem().ItemName == "bb_rnd_tele")
            {
                ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.EXPLORE_FIND_ITEM, 2049);
            }
            else if (RoomItem.GetBaseItem().ItemName.Contains("es_gate_"))
            {
                ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.EXPLORE_FIND_ITEM, 2167);
            }
            else if (RoomItem.GetBaseItem().ItemName.Contains("es_score_"))
            {
                ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.EXPLORE_FIND_ITEM, 2172);
            }
            else if (RoomItem.GetBaseItem().ItemName.Contains("es_exit"))
            {
                ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.EXPLORE_FIND_ITEM, 2166);
            }
            else if (RoomItem.GetBaseItem().ItemName == "es_tagging")
            {
                ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.EXPLORE_FIND_ITEM, 2148);
            }

            bool UserHasRights = false;
            if (room.CheckRights(Session))
            {
                UserHasRights = true;
            }

            int Request = Packet.PopInt();

            RoomItem.Interactor.OnTrigger(Session, RoomItem, Request, UserHasRights);
            RoomItem.OnTrigger(room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id));
        }
    }
}
