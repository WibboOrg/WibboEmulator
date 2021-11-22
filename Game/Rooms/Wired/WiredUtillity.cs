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
                case InteractionType.TRIGGER_SELF:
                case InteractionType.TRIGGER_COLLISION_USER:
                case InteractionType.TRIGGER_SCORE_ACHIEVED:
                case InteractionType.TRIGGER_STATE_CHANGED:
                case InteractionType.TRIGGER_WALK_ON_FURNI:
                case InteractionType.TRIGGER_WALK_OFF_FURNI:
                case InteractionType.TRIGGER_BOT_REACHED_AVTR:
                case InteractionType.TRIGGER_BOT_REACHED_STF:
                    return true;
                default:
                    return false;
            }
        }

        public static bool TypeIsWiredAction(InteractionType type)
        {
            switch (type)
            {
                case InteractionType.ACTION_GIVE_SCORE:
                case InteractionType.ACTION_POS_RESET:
                case InteractionType.ACTION_MOVE_ROTATE:
                case InteractionType.ACTION_RESET_TIMER:
                case InteractionType.ACTIONSHOWMESSAGE:
                case InteractionType.HIGHSCORE:
                case InteractionType.HIGHSCOREPOINTS:
                case InteractionType.ACTION_SUPER_WIRED:
                case InteractionType.ACTION_KICK_USER:
                case InteractionType.ACTION_TELEPORT_TO:
                case InteractionType.ACTION_ENDGAME_TEAM:
                case InteractionType.ACTION_TOGGLE_STATE:
                case InteractionType.ACTION_CALL_STACKS:
                case InteractionType.ACTION_FLEE:
                case InteractionType.ACTION_CHASE:
                case InteractionType.ACTION_COLLISION_CASE:
                case InteractionType.ACTION_COLLISION_TEAM:
                case InteractionType.ACTION_GIVE_REWARD:
                case InteractionType.ACTION_MOVE_TO_DIR:
                case InteractionType.ACTION_BOT_CLOTHES:
                case InteractionType.ACTION_BOT_TELEPORT:
                case InteractionType.ACTION_BOT_FOLLOW_AVATAR:
                case InteractionType.ACTION_BOT_GIVE_HANDITEM:
                case InteractionType.ACTION_BOT_MOVE:
                case InteractionType.ACTION_USER_MOVE:
                case InteractionType.ACTION_BOT_TALK_TO_AVATAR:
                case InteractionType.ACTION_BOT_TALK:
                case InteractionType.ACTION_JOIN_TEAM:
                case InteractionType.ACTION_LEAVE_TEAM:
                case InteractionType.ACTION_GIVE_SCORE_TM:
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
                case InteractionType.CONDITION_FURNIS_HAVE_NO_USERS:
                case InteractionType.CONDITION_STATE_POS:
                case InteractionType.CONDITION_STUFF_IS:
                case InteractionType.CONDITION_NOT_STUFF_IS:
                case InteractionType.CONDITION_STATE_POS_NEGATIVE:
                case InteractionType.CONDITION_DATE_RNG_ACTIVE:
                case InteractionType.CONDITION_TIME_LESS_THAN:
                case InteractionType.CONDITION_TIME_MORE_THAN:
                case InteractionType.CONDITION_TRIGGER_ON_FURNI:
                case InteractionType.CONDITION_TRIGGER_ON_FURNI_NEGATIVE:
                case InteractionType.CONDITION_HAS_FURNI_ON_FURNI:
                case InteractionType.CONDITION_HAS_FURNI_ON_FURNI_NEGATIVE:
                case InteractionType.CONDITION_ACTOR_IN_GROUP:
                case InteractionType.CONDITION_NOT_IN_GROUP:
                case InteractionType.CONDITION_SUPER_WIRED:
                case InteractionType.CONDITION_HAS_HANDITEM:
                case InteractionType.CONDITION_ACTOR_IN_TEAM:
                case InteractionType.CONDITION_NOT_IN_TEAM:
                case InteractionType.CONDITION_NOT_USER_COUNT:
                case InteractionType.CONDITION_USER_COUNT_IN:
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
