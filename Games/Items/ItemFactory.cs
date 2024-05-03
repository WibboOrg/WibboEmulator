namespace WibboEmulator.Games.Items;
using WibboEmulator.Database.Daos.Item;
using System.Data;
using WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.Users;

public class ItemFactory
{
    public static Item CreateSingleItemNullable(IDbConnection dbClient, ItemData data, User user, string extraData, int limitedNumber = 0, int limitedStack = 0)
    {
        if (data == null)
        {
            throw new InvalidOperationException("Data cannot be null.");
        }

        var item = new Item(0, 0, data.Id, extraData, limitedNumber, limitedStack, 0, 0, 0, 0, "", null)
        {
            Id = ItemDao.Insert(dbClient, data.Id, user.Id, extraData)
        };

        if (limitedNumber > 0)
        {
            ItemLimitedDao.Insert(dbClient, item.Id, limitedNumber, limitedStack);
        }

        return item;
    }

    public static Item CreateSingleItem(IDbConnection dbClient, ItemData data, User user, string extraData, int itemId, int limitedNumber = 0, int limitedStack = 0)
    {
        if (data == null || itemId <= 0)
        {
            return null;
        }

        ItemDao.Insert(dbClient, itemId, data.Id, user.Id, extraData);

        if (limitedNumber > 0)
        {
            ItemLimitedDao.Insert(dbClient, itemId, limitedNumber, limitedStack);
        }

        var item = new Item(itemId, 0, data.Id, extraData, limitedNumber, limitedStack, 0, 0, 0, 0, "", null);
        return item;
    }

    public static List<Item> CreateMultipleItems(IDbConnection dbClient, ItemData data, User user, string extraData, int amount)
    {
        if (data == null)
        {
            throw new InvalidOperationException("Data cannot be null.");
        }

        var items = new List<Item>();
        for (var i = 0; i < amount; i++)
        {
            var itemId = ItemDao.Insert(dbClient, data.Id, user.Id, extraData);

            var item = new Item(itemId, 0, data.Id, extraData, 0, 0, 0, 0, 0, 0, "", null);

            items.Add(item);
        }
        return items;
    }

    public static List<Item> CreateTeleporterItems(IDbConnection dbClient, ItemData data, User user)
    {
        var items = new List<Item>();

        var item1Id = ItemDao.Insert(dbClient, data.Id, user.Id, "");
        var item2Id = ItemDao.Insert(dbClient, data.Id, user.Id, item1Id.ToString());

        var item1 = new Item(item1Id, 0, data.Id, "", 0, 0, 0, 0, 0, 0, "", null);
        var item2 = new Item(item2Id, 0, data.Id, "", 0, 0, 0, 0, 0, 0, "", null);

        ItemTeleportDao.Insert(dbClient, item1Id, item2Id);
        ItemTeleportDao.Insert(dbClient, item2Id, item1Id);

        items.Add(item1);
        items.Add(item2);

        return items;
    }

    public static FurniInteractor CreateInteractor(Item item) => item.ItemData.InteractionType switch
    {
        InteractionType.GATE or InteractionType.BANZAI_PYRAMID => new InteractorGate(),
        InteractionType.SCORE_BOARD => new InteractorScoreboard(),
        InteractionType.VENDING_MACHINE => new InteractorVendor(),
        InteractionType.VENDING_ENABLE_MACHINE => new InteractorVendorEnable(),
        InteractionType.ALERT => new InteractorAlert(),
        InteractionType.ONEWAYGATE => new InteractorOneWayGate(),
        InteractionType.LOVESHUFFLER => new InteractorLoveShuffler(),
        InteractionType.HABBOWHEEL => new InteractorHabboWheel(),
        InteractionType.DICE => new InteractorDice(),
        InteractionType.BOTTLE => new InteractorSpinningBottle(),
        InteractionType.TELEPORT_ARROW or InteractionType.TELEPORT => new InteractorTeleport(),
        InteractionType.FOOTBALL => new InteractorFootball(),
        InteractionType.FOOTBALL_COUNTER_GREEN or InteractionType.FOOTBALL_COUNTER_YELLOW or InteractionType.FOOTBALL_COUNTER_BLUE or InteractionType.FOOTBALL_COUNTER_RED => new InteractorScoreCounter(),
        InteractionType.BANZAI_SCORE_BLUE or InteractionType.BANZAI_SCORE_RED or InteractionType.BANZAI_SCORE_YELLOW or InteractionType.BANZAI_SCORE_GREEN => new InteractorBanzaiScoreCounter(),
        InteractionType.CHRONO_TIMER => new InteractorTimer(),
        InteractionType.BANZAI_BLOB_2 or InteractionType.BANZAI_BLOB => new InteractorBlob(),
        InteractionType.BANZAI_PUCK => new InteractorBanzaiPuck(),
        InteractionType.FREEZE_TILE_BLOCK => new InteractorFreezeBlock(),
        InteractionType.FREEZE_TILE => new InteractorFreezeTile(),
        InteractionType.JUKEBOX => new InteractorJukebox(),
        InteractionType.TRIGGER_ONCE or InteractionType.CONDITION_COMPARE_HIGHSCORE or InteractionType.ACTION_RESET_POINTS_HIGHSCORE or InteractionType.ACTION_GIVE_POINTS_HIGHSCORE or InteractionType.ACTION_TRIDIMENSION or InteractionType.ACTION_TELEPORT_FURNI or InteractionType.ACTION_ROOM_MESSAGE or InteractionType.TRIGGER_AVATAR_ENTERS_ROOM or InteractionType.TRIGGER_GAME_ENDS or InteractionType.TRIGGER_GAME_STARTS or InteractionType.TRIGGER_PERIODICALLY or InteractionType.TRIGGER_PERIODICALLY_LONG or InteractionType.TRIGGER_AVATAR_SAYS_SOMETHING or InteractionType.TRIGGER_COMMAND or InteractionType.TRIGGER_COMMAND_SELF or InteractionType.TRIGGER_COLLISION_USER or InteractionType.TRIGGER_COLLISION_USER_SELF or InteractionType.TRIGGER_SCORE_ACHIEVED or InteractionType.TRIGGER_USER_CLICK or InteractionType.TRIGGER_USER_CLICK_SELF or InteractionType.TRIGGER_STATE_CHANGED or InteractionType.TRIGGER_WALK_ON_FURNI or InteractionType.TRIGGER_WALK_OFF_FURNI or InteractionType.TRIGGER_COLLISION or InteractionType.TRIGGER_AVATAR_EXIT or InteractionType.ACTION_GIVE_SCORE or InteractionType.ACTION_POS_RESET or InteractionType.ACTION_MOVE_ROTATE or InteractionType.ACTION_RESET_TIMER or InteractionType.ACTION_SHOW_MESSAGE or InteractionType.ACTION_GIVE_REWARD or InteractionType.ACTION_SUPER_WIRED or InteractionType.CONDITION_SUPER_WIRED or InteractionType.ACTION_TELEPORT_TO or InteractionType.ACTION_ENDGAME_TEAM or InteractionType.ACTION_CALL_STACKS or InteractionType.ACTION_TOGGLE_STATE or InteractionType.ACTION_KICK_USER or InteractionType.ACTION_FLEE or InteractionType.ACTION_CHASE or InteractionType.ACTION_COLLISION_CASE or InteractionType.ACTION_COLLISION_ITEM or InteractionType.ACTION_COLLISION_TEAM or InteractionType.ACTION_MOVE_TO_DIR or InteractionType.CONDITION_FURNIS_HAVE_USERS or InteractionType.CONDITION_FURNIS_HAVE_NO_USERS or InteractionType.CONDITION_HAS_FURNI_ON_FURNI or InteractionType.CONDITION_HAS_FURNI_ON_FURNI_NEGATIVE or InteractionType.CONDITION_STATE_POS or InteractionType.CONDITION_STUFF_IS or InteractionType.CONDITION_NOT_STUFF_IS or InteractionType.CONDITION_DATE_RNG_ACTIVE or InteractionType.CONDITION_STATE_POS_NEGATIVE or InteractionType.CONDITION_TIME_LESS_THAN or InteractionType.CONDITION_TIME_MORE_THAN or InteractionType.CONDITION_TRIGGER_ON_FURNI or InteractionType.CONDITION_TRIGGER_ON_FURNI_NEGATIVE or InteractionType.CONDITION_ACTOR_IN_GROUP or InteractionType.CONDITION_NOT_IN_GROUP or InteractionType.TRIGGER_BOT_REACHED_STF or InteractionType.TRIGGER_BOT_REACHED_AVTR or InteractionType.ACTION_BOT_CLOTHES or InteractionType.ACTION_BOT_TELEPORT or InteractionType.ACTION_BOT_FOLLOW_AVATAR or InteractionType.ACTION_BOT_GIVE_HANDITEM or InteractionType.ACTION_BOT_MOVE or InteractionType.ACTION_USER_MOVE or InteractionType.ACTION_BOT_TALK_TO_AVATAR or InteractionType.ACTION_BOT_TALK or InteractionType.CONDITION_HAS_HANDITEM or InteractionType.ACTION_JOIN_TEAM or InteractionType.ACTION_LEAVE_TEAM or InteractionType.ACTION_GIVE_SCORE_TM or InteractionType.CONDITION_ACTOR_IN_TEAM or InteractionType.CONDITION_NOT_IN_TEAM or InteractionType.CONDITION_NOT_USER_COUNT or InteractionType.CONDITION_USER_COUNT_IN or InteractionType.CONDITION_COLLISION_IS or InteractionType.CONDITION_NOT_COLLISION_IS => new InteractorWired(),
        InteractionType.MANNEQUIN => new InteractorManiqui(),
        InteractionType.TONER => new InteractorChangeBackgrounds(),
        InteractionType.PUZZLE_BOX => new InteractorPuzzleBox(),
        InteractionType.FLOOR_SWITCH => new InteractorSwitch(item.ItemData.Modes),
        InteractionType.CRACKABLE => new InteractorCrackable(item.ItemData.Modes),
        InteractionType.TV_YOUTUBE => new InteractorTvYoutube(),
        InteractionType.LOVELOCK => new InteractorLoveLock(),
        InteractionType.PHOTO => new InteractorIgnore(),
        InteractionType.EXCHANGE_TREE or InteractionType.EXCHANGE_TREE_CLASSIC or InteractionType.EXCHANGE_TREE_EPIC or InteractionType.EXCHANGE_TREE_LEGEND => new InteractorExchangeTree(),
        InteractionType.PREMIUM_CLASSIC or InteractionType.PREMIUM_EPIC or InteractionType.PREMIUM_LEGEND => new InteractorPremiumBox(),
        InteractionType.BANZAI_TELE => new InteractorBanzaiTele(),
        InteractionType.BADGE_TROC => new InteractorBadgeTroc(),
        InteractionType.TROC_BANNER => new InteractorBannerTroc(),
        InteractionType.GIFT_BANNER => new InteractorBannerGift(),
        InteractionType.HIGH_SCORE or InteractionType.HIGH_SCORE_POINTS => new InteractorGenericSwitch(2),
        _ => new InteractorGenericSwitch(item.ItemData.Modes),
    };
}
