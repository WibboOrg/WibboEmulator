namespace WibboEmulator.Games.Items.Wired;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Actions;
using WibboEmulator.Games.Items.Wired.Conditions;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Items.Wired.Triggers;
using WibboEmulator.Games.Rooms;

public class WiredRegister
{
    private static IWired GetWiredHandler(Item item, Room room)
    {
        IWired handler = null;
        switch (item.GetBaseItem().InteractionType)
        {
            case InteractionType.TRIGGER_ONCE:
                handler = new TimerTrigger(item, room);
                break;
            case InteractionType.TRIGGER_AVATAR_ENTERS_ROOM:
                handler = new EntersRoom(item, room);
                break;
            case InteractionType.TRIGGER_COLLISION:
                handler = new Collision(item, room);
                break;
            case InteractionType.TRIGGER_GAME_ENDS:
                handler = new GameEnds(item, room);
                break;
            case InteractionType.TRIGGER_GAME_STARTS:
                handler = new GameStarts(item, room);
                break;
            case InteractionType.TRIGGER_PERIODICALLY:
                handler = new Repeater(item, room);
                break;
            case InteractionType.TRIGGER_PERIODICALLY_LONG:
                handler = new Repeaterlong(item, room);
                break;
            case InteractionType.TRIGGER_AVATAR_SAYS_SOMETHING:
                handler = new UserSays(item, room);
                break;
            case InteractionType.TRIGGER_COMMAND:
                handler = new UserCommand(item, room);
                break;
            case InteractionType.TRIGGER_SELF:
                handler = new UserTriggerSelf(item, room);
                break;
            case InteractionType.TRIGGER_BOT_REACHED_AVTR:
                handler = new BotReadchedAvatar(item, room);
                break;
            case InteractionType.TRIGGER_COLLISION_USER:
                handler = new UserCollision(item, room);
                break;
            case InteractionType.TRIGGER_SCORE_ACHIEVED:
                handler = new ScoreAchieved(item, room);
                break;
            case InteractionType.TRIGGER_STATE_CHANGED:
                handler = new SateChanged(item, room);
                break;
            case InteractionType.TRIGGER_WALK_ON_FURNI:
                handler = new WalksOnFurni(item, room);
                break;
            case InteractionType.TRIGGER_WALK_OFF_FURNI:
                handler = new WalksOffFurni(item, room);
                break;

            case InteractionType.ACTION_GIVE_SCORE:
                handler = new GiveScore(item, room);
                break;
            case InteractionType.ACTION_GIVE_SCORE_TM:
                handler = new GiveScoreTeam(item, room);
                break;
            case InteractionType.ACTION_POS_RESET:
                handler = new PositionReset(item, room);
                break;
            case InteractionType.ACTION_MOVE_ROTATE:
                handler = new MoveRotate(item, room);
                break;
            case InteractionType.ACTION_RESET_TIMER:
                handler = new TimerReset(item, room);
                break;
            case InteractionType.ACTIONSHOWMESSAGE:
                handler = new ShowMessage(item, room);
                break;
            case InteractionType.ACTION_SUPER_WIRED:
                handler = new SuperWired(item, room);
                break;
            case InteractionType.ACTION_KICK_USER:
                handler = new KickUser(item, room);
                break;
            case InteractionType.ACTION_TELEPORT_TO:
                handler = new TeleportToItem(item, room);
                break;
            case InteractionType.ACTION_ENDGAME_TEAM:
                handler = new TeamGameOver(item, room);
                break;
            case InteractionType.ACTION_TOGGLE_STATE:
                handler = new ToggleItemState(item, room);
                break;
            case InteractionType.ACTION_CALL_STACKS:
                handler = new ExecutePile(item, room);
                break;
            case InteractionType.ACTION_FLEE:
                handler = new Escape(item, room);
                break;
            case InteractionType.ACTION_CHASE:
                handler = new Chase(item, room);
                break;
            case InteractionType.ACTION_COLLISION_TEAM:
                handler = new CollisionTeam(item, room);
                break;
            case InteractionType.ACTION_COLLISION_CASE:
                handler = new CollisionCase(item, room);
                break;
            case InteractionType.ACTION_MOVE_TO_DIR:
                handler = new MoveToDir(item, room);
                break;
            case InteractionType.ACTION_BOT_CLOTHES:
                handler = new BotClothes(item, room);
                break;
            case InteractionType.ACTION_BOT_TELEPORT:
                handler = new BotTeleport(item, room);
                break;
            case InteractionType.ACTION_BOT_FOLLOW_AVATAR:
                handler = new BotFollowAvatar(item, room);
                break;
            case InteractionType.ACTION_BOT_GIVE_HANDITEM:
                handler = new BotGiveHanditem(item, room);
                break;
            case InteractionType.ACTION_BOT_MOVE:
                handler = new BotMove(item, room);
                break;
            case InteractionType.ACTION_USER_MOVE:
                handler = new UserMove(item, room);
                break;
            case InteractionType.ACTION_BOT_TALK_TO_AVATAR:
                handler = new BotTalkToAvatar(item, room);
                break;
            case InteractionType.ACTION_BOT_TALK:
                handler = new BotTalk(item, room);
                break;
            case InteractionType.ACTION_LEAVE_TEAM:
                handler = new TeamLeave(item, room);
                break;
            case InteractionType.ACTION_JOIN_TEAM:
                handler = new TeamJoin(item, room);
                break;
            case InteractionType.HIGHSCORE:
                handler = new HighScore(item, room);
                break;
            case InteractionType.HIGHSCOREPOINTS:
                handler = new HighScorePoints(item, room);
                break;

            case InteractionType.CONDITION_SUPER_WIRED:
                handler = new SuperWiredCondition(item, room);
                break;
            case InteractionType.CONDITION_FURNIS_HAVE_USERS:
                handler = new FurniHasUser(item, room);
                break;
            case InteractionType.CONDITION_FURNIS_HAVE_NO_USERS:
                handler = new FurniHasNoUser(item, room);
                break;
            case InteractionType.CONDITION_STATE_POS:
                handler = new FurniStatePosMatch(item, room);
                break;
            case InteractionType.CONDITION_STUFF_IS:
                handler = new FurniStuffIs(item, room);
                break;
            case InteractionType.CONDITION_NOT_STUFF_IS:
                handler = new FurniNotStuffIs(item, room);
                break;
            case InteractionType.CONDITION_DATE_RNG_ACTIVE:
                handler = new DateRangeActive(item, room);
                break;
            case InteractionType.CONDITION_STATE_POS_NEGATIVE:
                handler = new FurniStatePosMatchNegative(item, room);
                break;
            case InteractionType.CONDITION_TIME_LESS_THAN:
                handler = new LessThanTimer(item, room);
                break;
            case InteractionType.CONDITION_TIME_MORE_THAN:
                handler = new MoreThanTimer(item, room);
                break;
            case InteractionType.CONDITION_TRIGGER_ON_FURNI:
                handler = new TriggerUserIsOnFurni(item, room);
                break;
            case InteractionType.CONDITION_TRIGGER_ON_FURNI_NEGATIVE:
                handler = new TriggerUserIsOnFurniNegative(item, room);
                break;
            case InteractionType.CONDITION_HAS_FURNI_ON_FURNI:
                handler = new HasFurniOnFurni(item, room);
                break;
            case InteractionType.CONDITION_HAS_FURNI_ON_FURNI_NEGATIVE:
                handler = new HasFurniOnFurniNegative(item, room);
                break;
            case InteractionType.CONDITION_ACTOR_IN_GROUP:
                handler = new HasUserInGroup(item, room);
                break;
            case InteractionType.CONDITION_NOT_USER_COUNT:
                handler = new RoomUserNotCount(item, room);
                break;
            case InteractionType.CONDITION_USER_COUNT_IN:
                handler = new RoomUserCount(item, room);
                break;
            case InteractionType.CONDITION_NOT_IN_GROUP:
                handler = new HasUserNotInGroup(item, room);
                break;
            case InteractionType.CONDITION_ACTOR_IN_TEAM:
                handler = new ActorInTeam(item, room);
                break;
            case InteractionType.CONDITION_NOT_IN_TEAM:
                handler = new ActorNotInTeam(item, room);
                break;
        }

        return handler;
    }

    internal static void HandleRegister(Item item, Room room, List<int> intParams, string stringParam, List<int> stuffIds, int selectionCode, int delay, bool isStaff, bool isGod)
    {
        var handler = GetWiredHandler(item, room);

        if (handler != null)
        {
            handler.Init(intParams, stringParam, stuffIds, selectionCode, delay, isStaff, isGod);
            handler.LoadItems();

            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                handler.SaveToDatabase(dbClient);
            }

            HandleSave(handler, item);
        }
    }

    public static void HandleRegister(Room room, Item item)
    {
        var handler = GetWiredHandler(item, room);

        if (handler != null)
        {
            handler.LoadItems();

            HandleSave(handler, item);
        }
    }

    public static void HandleRegister(Item item, Room room, IQueryAdapter dbClient)
    {
        var handler = GetWiredHandler(item, room);

        if (handler != null)
        {
            var row = ItemWiredDao.GetOne(dbClient, item.Id);
            if (row != null)
            {
                handler.LoadFromDatabase(row);
            }

            handler.LoadItems(true);

            HandleSave(handler, item);
        }
    }

    private static void HandleSave(IWired handler, Item item)
    {
        if (item.WiredHandler != null)
        {
            item.WiredHandler.Dispose();
            item.WiredHandler = null;
        }

        item.WiredHandler = handler;
    }
}
