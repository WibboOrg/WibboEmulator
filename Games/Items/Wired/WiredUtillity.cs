namespace WibboEmulator.Games.Items.Wired;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Rooms;

public class WiredUtillity
{
    public static bool TypeIsWiredTrigger(InteractionType type) => type switch
    {
        InteractionType.TRIGGER_SAYS_COMMAND_TRANSFERT or InteractionType.TRIGGER_SAYS_COMMAND_RECOVER or InteractionType.TRIGGER_COLLISION or InteractionType.TRIGGER_ONCE or InteractionType.TRIGGER_AVATAR_ENTERS_ROOM or InteractionType.TRIGGER_GAME_ENDS or InteractionType.TRIGGER_GAME_STARTS or InteractionType.TRIGGER_PERIODICALLY or InteractionType.TRIGGER_PERIODICALLY_LONG or InteractionType.TRIGGER_AVATAR_SAYS_SOMETHING or InteractionType.TRIGGER_COMMAND or InteractionType.TRIGGER_COMMAND_SELF or InteractionType.TRIGGER_COLLISION_USER or InteractionType.TRIGGER_COLLISION_USER_SELF or InteractionType.TRIGGER_SCORE_ACHIEVED or InteractionType.TRIGGER_STATE_CHANGED or InteractionType.TRIGGER_WALK_ON_FURNI or InteractionType.TRIGGER_WALK_OFF_FURNI or InteractionType.TRIGGER_BOT_REACHED_AVTR or InteractionType.TRIGGER_BOT_REACHED_STF or InteractionType.TRIGGER_AVATAR_EXIT or InteractionType.TRIGGER_USER_CLICK or InteractionType.TRIGGER_USER_CLICK_SELF => true,
        _ => false,
    };

    public static bool TypeIsWiredAction(InteractionType type) => type switch
    {
        InteractionType.ACTION_TRIDIMENSION or InteractionType.ACTION_TELEPORT_FURNI or InteractionType.ACTION_ROOM_MESSAGE or InteractionType.ACTION_GIVE_SCORE or InteractionType.ACTION_POS_RESET or InteractionType.ACTION_MOVE_ROTATE or InteractionType.ACTION_RESET_TIMER or InteractionType.ACTION_SHOW_MESSAGE or InteractionType.HIGH_SCORE or InteractionType.HIGH_SCORE_POINTS or InteractionType.ACTION_SUPER_WIRED or InteractionType.ACTION_KICK_USER or InteractionType.ACTION_TELEPORT_TO or InteractionType.ACTION_ENDGAME_TEAM or InteractionType.ACTION_TOGGLE_STATE or InteractionType.ACTION_CALL_STACKS or InteractionType.ACTION_FLEE or InteractionType.ACTION_CHASE or InteractionType.ACTION_COLLISION_CASE or InteractionType.ACTION_COLLISION_ITEM or InteractionType.ACTION_COLLISION_TEAM or InteractionType.ACTION_GIVE_REWARD or InteractionType.ACTION_MOVE_TO_DIR or InteractionType.ACTION_BOT_CLOTHES or InteractionType.ACTION_BOT_TELEPORT or InteractionType.ACTION_BOT_FOLLOW_AVATAR or InteractionType.ACTION_BOT_GIVE_HANDITEM or InteractionType.ACTION_BOT_MOVE or InteractionType.ACTION_USER_MOVE or InteractionType.ACTION_BOT_TALK_TO_AVATAR or InteractionType.ACTION_BOT_TALK or InteractionType.ACTION_JOIN_TEAM or InteractionType.ACTION_LEAVE_TEAM or InteractionType.ACTION_GIVE_SCORE_TM => true,
        _ => false,
    };

    public static bool TypeIsWiredCondition(InteractionType type) => type switch
    {
        InteractionType.CONDITION_FURNIS_HAVE_USERS or InteractionType.CONDITION_FURNIS_HAVE_NO_USERS or InteractionType.CONDITION_STATE_POS or InteractionType.CONDITION_STUFF_IS or InteractionType.CONDITION_NOT_STUFF_IS or InteractionType.CONDITION_STATE_POS_NEGATIVE or InteractionType.CONDITION_DATE_RNG_ACTIVE or InteractionType.CONDITION_TIME_LESS_THAN or InteractionType.CONDITION_TIME_MORE_THAN or InteractionType.CONDITION_TRIGGER_ON_FURNI or InteractionType.CONDITION_TRIGGER_ON_FURNI_NEGATIVE or InteractionType.CONDITION_HAS_FURNI_ON_FURNI or InteractionType.CONDITION_HAS_FURNI_ON_FURNI_NEGATIVE or InteractionType.CONDITION_ACTOR_IN_GROUP or InteractionType.CONDITION_NOT_IN_GROUP or InteractionType.CONDITION_SUPER_WIRED or InteractionType.CONDITION_HAS_HANDITEM or InteractionType.CONDITION_ACTOR_IN_TEAM or InteractionType.CONDITION_NOT_IN_TEAM or InteractionType.CONDITION_NOT_USER_COUNT or InteractionType.CONDITION_USER_COUNT_IN or InteractionType.CONDITION_COLLISION_IS or InteractionType.CONDITION_NOT_COLLISION_IS => true,
        _ => false,
    };

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
        else if (type == InteractionType.SPECIAL_RANDOM)
        {
            return true;
        }
        else if (type == InteractionType.SPECIAL_UNSEEN)
        {
            return true;
        }
        else if (type == InteractionType.SPECIAL_ANIMATE)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool AllowHideWiredType(InteractionType type)
    {
        if (type is InteractionType.HIGH_SCORE or InteractionType.HIGH_SCORE_POINTS)
        {
            return false;
        }

        if (type is InteractionType.WIRED_ITEM)
        {
            return true;
        }

        if (TypeIsWired(type))
        {
            return true;
        }

        return false;
    }

    public static void SaveTriggerItem(IQueryAdapter dbClient, int triggerId, string triggerData2, string triggerData, bool allUsertriggerable, List<Item> itemslist, int delay = 0)
    {
        var triggerItems = "";

        if (itemslist != null)
        {
            foreach (var item in itemslist)
            {
                triggerItems += item.Id + ";";
            }
        }

        triggerItems = triggerItems.TrimEnd(';');

        ItemWiredDao.Delete(dbClient, triggerId);
        ItemWiredDao.Insert(dbClient, triggerId, triggerData, triggerData2, allUsertriggerable, triggerItems, delay);
    }

    public static void ParseMessage(RoomUser user, Room room, ref string textMessage)
    {
        if (user != null)
        {
            textMessage = textMessage.Replace("#username#", user.GetUsername());
            textMessage = textMessage.Replace("#point#", user.WiredPoints.ToString());

            if (user.Client != null)
            {
                textMessage = textMessage.Replace("#wpcount#", user.Client.User != null ? user.Client.User.WibboPoints.ToString() : "0");
            }

            if (user.Roleplayer != null)
            {
                textMessage = textMessage.Replace("#money#", user.Roleplayer.Money.ToString());
            }
        }

        if (room != null)
        {
            textMessage = textMessage.Replace("#roomname#", room.RoomData.Name.ToString());
            textMessage = textMessage.Replace("#vote_yes#", room.VotedYesCount.ToString());
            textMessage = textMessage.Replace("#vote_no#", room.VotedNoCount.ToString());
        }
    }
}

public class ItemsPosReset
{
    public int Id { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public double Z { get; set; }
    public int Rot { get; set; }
    public string ExtraData { get; set; }

    public ItemsPosReset(int id, int x, int y, double z, int rot, string extraData)
    {
        this.Id = id;
        this.X = x;
        this.Y = y;
        this.Z = z;
        this.Rot = rot;
        this.ExtraData = extraData;
    }
}
