using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Items;
using System.Collections.Generic;

namespace Butterfly.Game.Rooms.Wired
{
    public class WiredUtillity
    {
        public static bool TypeIsWiredTrigger(InteractionType type)
        {
            switch (type)
            {
                case InteractionType.TRIGGER_COLLISION:
                case InteractionType.TRIGGER_ONCE:
                case InteractionType.TRIGGER_AVATAR_ENTERS_ROOM:
                case InteractionType.TRIGGER_GAME_ENDS:
                case InteractionType.TRIGGER_GAME_STARTS:
                case InteractionType.TRIGGER_PERIODICALLY:
                case InteractionType.TRIGGER_PERIODICALLY_LONG:
                case InteractionType.TRIGGER_AVATAR_SAYS_SOMETHING:
                case InteractionType.TRIGGER_COMMAND:
                case InteractionType.WIRED_TRIGGER_SELF:
                case InteractionType.TRIGGER_COLLISION_USER:
                case InteractionType.TRIGGER_SCORE_ACHIEVED:
                case InteractionType.TRIGGER_STATE_CHANGED:
                case InteractionType.TRIGGER_WALK_ON_FURNI:
                case InteractionType.TRIGGER_WALK_OFF_FURNI:
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
                case InteractionType.CONDITION_FURNIS_HAVE_USERS:
                case InteractionType.CONDITIONFURNISHAVENOUSERS:
                case InteractionType.CONDITIONSTATEPOS:
                case InteractionType.WF_CND_STUFF_IS:
                case InteractionType.WF_CND_NOT_STUFF_IS:
                case InteractionType.CONDITIONSTATEPOSNEGATIVE:
                case InteractionType.CONDITION_DATE_RNG_ACTIVE:
                case InteractionType.CONDITIONTIMELESSTHAN:
                case InteractionType.CONDITIONTIMEMORETHAN:
                case InteractionType.CONDITIONTRIGGERONFURNI:
                case InteractionType.CONDITIONTRIGGERONFURNINEGATIVE:
                case InteractionType.CONDITIONHASFURNIONFURNI:
                case InteractionType.CONDITIONHASFURNIONFURNINEGATIVE:
                case InteractionType.CONDITIONACTORINGROUP:
                case InteractionType.CONDITIONNOTINGROUP:
                case InteractionType.CONDITION_SUPER_WIRED:
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

        public static void SaveTriggerItem(IQueryAdapter dbClient, int triggerId, string triggerData2, string triggerData, bool allUsertriggerable, List<Item> itemslist)
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

            ItemWiredDao.Delete(dbClient, triggerId);
            ItemWiredDao.Insert(dbClient, triggerId, triggerData, triggerData2, allUsertriggerable, triggersitem);
        }
    }
}
