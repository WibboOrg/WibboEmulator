using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.Items;
using System.Collections.Generic;

namespace Butterfly.HabboHotel.Rooms.Wired
{
    public class WiredUtillity
    {
        public static bool TypeIsWiredTrigger(InteractionType type)
        {
            switch (type)
            {
                case InteractionType.TRIGGERCOLLISION:
                case InteractionType.TRIGGERTIMER:
                case InteractionType.TRIGGERROOMENTER:
                case InteractionType.TRIGGERGAMEEND:
                case InteractionType.TRIGGERGAMESTART:
                case InteractionType.TRIGGERREPEATER:
                case InteractionType.TRIGGERREPEATERLONG:
                case InteractionType.TRIGGERONUSERSAY:
                case InteractionType.TRIGGERCOMMAND:
                case InteractionType.WIRED_TRIGGER_SELF:
                case InteractionType.TRIGGERCOLLISIONUSER:
                case InteractionType.TRIGGERSCOREACHIEVED:
                case InteractionType.TRIGGERSTATECHANGED:
                case InteractionType.TRIGGERWALKONFURNI:
                case InteractionType.TRIGGERWALKOFFFURNI:
                case InteractionType.WF_TRG_BOT_REACHED_AVTR:
                case InteractionType.WF_TRG_BOT_REACHED_STF:
                    return true;
                default:
                    return false;
            }
        }

        public static bool TypeIsWiredAction(InteractionType type)
        {
            switch (type)
            {
                case InteractionType.ACTIONGIVESCORE:
                case InteractionType.ACTIONPOSRESET:
                case InteractionType.ACTIONMOVEROTATE:
                case InteractionType.ACTIONRESETTIMER:
                case InteractionType.ACTIONSHOWMESSAGE:
                case InteractionType.HIGHSCORE:
                case InteractionType.HIGHSCOREPOINTS:
                case InteractionType.SUPERWIRED:
                case InteractionType.ACTIONKICKUSER:
                case InteractionType.ACTIONTELEPORTTO:
                case InteractionType.WF_ACT_ENDGAME_TEAM:
                case InteractionType.ACTIONTOGGLESTATE:
                case InteractionType.WF_ACT_CALL_STACKS:
                case InteractionType.ACTIONFLEE:
                case InteractionType.ACTIONCHASE:
                case InteractionType.COLLISIONCASE:
                case InteractionType.COLLISIONTEAM:
                case InteractionType.ACTIONGIVEREWARD:
                case InteractionType.ACTIONMOVETODIR:
                case InteractionType.WF_ACT_BOT_CLOTHES:
                case InteractionType.WF_ACT_BOT_TELEPORT:
                case InteractionType.WF_ACT_BOT_FOLLOW_AVATAR:
                case InteractionType.WF_ACT_BOT_GIVE_HANDITEM:
                case InteractionType.WF_ACT_BOT_MOVE:
                case InteractionType.WF_ACT_USER_MOVE:
                case InteractionType.WF_ACT_BOT_TALK_TO_AVATAR:
                case InteractionType.WF_ACT_BOT_TALK:
                case InteractionType.WF_ACT_JOIN_TEAM:
                case InteractionType.WF_ACT_LEAVE_TEAM:
                case InteractionType.WF_ACT_GIVE_SCORE_TM:
                    return true;
                default:
                    return false;
            }
        }

        public static bool TypeIsWiredCondition(InteractionType type)
        {
            switch (type)
            {
                case InteractionType.CONDITIONFURNISHAVEUSERS:
                case InteractionType.CONDITIONFURNISHAVENOUSERS:
                case InteractionType.CONDITIONSTATEPOS:
                case InteractionType.WF_CND_STUFF_IS:
                case InteractionType.WF_CND_NOT_STUFF_IS:
                case InteractionType.CONDITIONSTATEPOSNEGATIVE:
                case InteractionType.CONDITIONTIMELESSTHAN:
                case InteractionType.CONDITIONTIMEMORETHAN:
                case InteractionType.CONDITIONTRIGGERONFURNI:
                case InteractionType.CONDITIONTRIGGERONFURNINEGATIVE:
                case InteractionType.CONDITIONHASFURNIONFURNI:
                case InteractionType.CONDITIONHASFURNIONFURNINEGATIVE:
                case InteractionType.CONDITIONACTORINGROUP:
                case InteractionType.CONDITIONNOTINGROUP:
                case InteractionType.SUPERWIREDCONDITION:
                case InteractionType.WF_CND_HAS_HANDITEM:
                case InteractionType.WF_CND_ACTOR_IN_TEAM:
                case InteractionType.WF_CND_NOT_IN_TEAM:
                case InteractionType.WF_CND_NOT_USER_COUNT:
                case InteractionType.WF_CND_USER_COUNT_IN:
                    return true;
                default:
                    return false;
            }
        }

        public static bool TypeIsWired(InteractionType type)
        {
            if (TypeIsWiredTrigger(type))
            {
                return true;
            }
            else if (TypeIsWiredAction(type))
            {
                return true;
            }
            else if (TypeIsWiredCondition(type))
            {
                return true;
            }
            else if (type == InteractionType.SPECIALRANDOM)
            {
                return true;
            }
            else if (type == InteractionType.SPECIALUNSEEN)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void SaveTriggerItem(IQueryAdapter dbClient, int triggerID, string triggerData2, string triggerData, bool allUsertriggerable, List<Item> itemslist)
        {
            string triggersitem = "";

            if (itemslist != null)
            {
                int i = 0;
                foreach (Item item in itemslist)
                {
                    if (i != 0)
                    {
                        triggersitem += ";";
                    }

                    triggersitem += item.Id;

                    i++;
                }
            }

            dbClient.RunQuery("DELETE FROM wired_items WHERE trigger_id = " + triggerID);
            dbClient.SetQuery("INSERT INTO wired_items (trigger_id, trigger_data, trigger_data_2, all_user_triggerable, triggers_item) VALUES (@id, @trigger_data, @trigger_data_2, @triggerable, @triggers_item)");
            dbClient.AddParameter("id", triggerID);
            dbClient.AddParameter("trigger_data", triggerData);
            dbClient.AddParameter("trigger_data_2", triggerData2);
            dbClient.AddParameter("triggerable", (allUsertriggerable ? 1 : 0));
            dbClient.AddParameter("triggers_item", triggersitem);
            dbClient.RunQuery();
        }
    }
}
