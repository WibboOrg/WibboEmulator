using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.Users;

namespace WibboEmulator.Games.Items
{
    public class ItemFactory
    {
        public static Item CreateSingleItemNullable(ItemData Data, User user, string ExtraData, int LimitedNumber = 0, int LimitedStack = 0)
        {
            if (Data == null)
            {
                throw new InvalidOperationException("Data cannot be null.");
            }

            Item Item = new Item(0, 0, Data.Id, ExtraData, LimitedNumber, LimitedStack, 0, 0, 0, 0, "", null);

            using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            Item.Id = ItemDao.Insert(dbClient, Data.Id, user.Id, ExtraData);

            if (LimitedNumber > 0)
            {
                ItemLimitedDao.Insert(dbClient, Item.Id, LimitedNumber, LimitedStack);
            }

            return Item;
        }

        public static Item CreateSingleItem(ItemData Data, User user, string ExtraData, int ItemId, int LimitedNumber = 0, int LimitedStack = 0)
        {
            if (Data == null)
            {
                return null;
            }

            int InsertId = 0;
            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                InsertId = ItemDao.Insert(dbClient, ItemId, Data.Id, user.Id, ExtraData);

                if (LimitedNumber > 0 && InsertId > 0)
                {
                    ItemLimitedDao.Insert(dbClient, ItemId, LimitedNumber, LimitedStack);
                }
            }

            if (InsertId <= 0)
            {
                return null;
            }

            Item Item = new Item(ItemId, 0, Data.Id, ExtraData, LimitedNumber, LimitedStack, 0, 0, 0, 0, "", null);
            return Item;
        }

        public static List<Item> CreateMultipleItems(ItemData Data, User user, string ExtraData, int Amount)
        {
            if (Data == null) throw new InvalidOperationException("Data cannot be null.");

            List<Item> Items = new List<Item>();

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                for (int i = 0; i < Amount; i++)
                {
                    int itemId = ItemDao.Insert(dbClient, Data.Id, user.Id, ExtraData);

                    Item Item = new Item(itemId, 0, Data.Id, ExtraData, 0, 0, 0, 0, 0, 0, "", null);

                    Items.Add(Item);
                }
            }
            return Items;
        }

        public static List<Item> CreateTeleporterItems(ItemData data, User user)
        {
            List<Item> Items = new List<Item>();

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                int Item1Id = ItemDao.Insert(dbClient, data.Id, user.Id, "");
                int Item2Id = ItemDao.Insert(dbClient, data.Id, user.Id, Item1Id.ToString());

                Item Item1 = new Item(Item1Id, 0, data.Id, "", 0, 0, 0, 0, 0, 0, "", null);
                Item Item2 = new Item(Item2Id, 0, data.Id, "", 0, 0, 0, 0, 0, 0, "", null);

                ItemTeleportDao.Insert(dbClient, Item1Id, Item2Id);
                ItemTeleportDao.Insert(dbClient, Item2Id, Item1Id);

                Items.Add(Item1);
                Items.Add(Item2);
            }
            return Items;
        }

        public static void CreateMoodlightData(Item Item)
        {
            using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            ItemMoodlightDao.Insert(dbClient, Item.Id);
        }

        public static FurniInteractor CreateInteractor(Item item)
        {
            switch (item.GetBaseItem().InteractionType)
            {
                case InteractionType.GATE:
                case InteractionType.BANZAIPYRAMID:
                    return new InteractorGate();
                case InteractionType.SCOREBOARD:
                    return new InteractorScoreboard();
                case InteractionType.VENDINGMACHINE:
                    return new InteractorVendor();
                case InteractionType.VENDINGENABLEMACHINE:
                    return new InteractorVendorEnable();
                case InteractionType.ALERT:
                    return new InteractorAlert();
                case InteractionType.ONEWAYGATE:
                    return new InteractorOneWayGate();
                case InteractionType.LOVESHUFFLER:
                    return new InteractorLoveShuffler();
                case InteractionType.HABBOWHEEL:
                    return new InteractorHabboWheel();
                case InteractionType.DICE:
                    return new InteractorDice();
                case InteractionType.BOTTLE:
                    return new InteractorSpinningBottle();
                case InteractionType.ARROW:
                case InteractionType.TELEPORT:
                    return new InteractorTeleport();
                case InteractionType.FOOTBALL:
                    return new InteractorFootball();
                case InteractionType.FOOTBALLCOUNTERGREEN:
                case InteractionType.FOOTBALLCOUNTERYELLOW:
                case InteractionType.FOOTBALLCOUNTERBLUE:
                case InteractionType.FOOTBALLCOUNTERRED:
                    return new InteractorScoreCounter();
                case InteractionType.BANZAISCOREBLUE:
                case InteractionType.BANZAISCORERED:
                case InteractionType.BANZAISCOREYELLOW:
                case InteractionType.BANZAISCOREGREEN:
                    return new InteractorBanzaiScoreCounter();
                case InteractionType.CHRONOTIMER:
                    return new InteractorTimer();
                case InteractionType.BANZAIBLO:
                case InteractionType.BANZAIBLOB:
                    return new InteractorBlob();
                case InteractionType.BANZAIPUCK:
                    return new InteractorBanzaiPuck();
                case InteractionType.FREEZETILEBLOCK:
                    return new InteractorFreezeBlock();
                case InteractionType.FREEZETILE:
                    return new InteractorFreezeTile();
                case InteractionType.JUKEBOX:
                    return new InteractorJukebox();
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
                case InteractionType.TRIGGER_COLLISION:
                case InteractionType.ACTION_GIVE_SCORE:
                case InteractionType.ACTION_POS_RESET:
                case InteractionType.ACTION_MOVE_ROTATE:
                case InteractionType.ACTION_RESET_TIMER:
                case InteractionType.ACTIONSHOWMESSAGE:
                case InteractionType.ACTION_GIVE_REWARD:
                case InteractionType.ACTION_SUPER_WIRED:
                case InteractionType.CONDITION_SUPER_WIRED:
                case InteractionType.ACTION_TELEPORT_TO:
                case InteractionType.ACTION_ENDGAME_TEAM:
                case InteractionType.ACTION_CALL_STACKS:
                case InteractionType.ACTION_TOGGLE_STATE:
                case InteractionType.ACTION_KICK_USER:
                case InteractionType.ACTION_FLEE:
                case InteractionType.ACTION_CHASE:
                case InteractionType.ACTION_COLLISION_CASE:
                case InteractionType.ACTION_COLLISION_TEAM:
                case InteractionType.ACTION_MOVE_TO_DIR:
                case InteractionType.CONDITION_FURNIS_HAVE_USERS:
                case InteractionType.CONDITION_FURNIS_HAVE_NO_USERS:
                case InteractionType.CONDITION_HAS_FURNI_ON_FURNI:
                case InteractionType.CONDITION_HAS_FURNI_ON_FURNI_NEGATIVE:
                case InteractionType.CONDITION_STATE_POS:
                case InteractionType.CONDITION_STUFF_IS:
                case InteractionType.CONDITION_NOT_STUFF_IS:
                case InteractionType.CONDITION_DATE_RNG_ACTIVE:
                case InteractionType.CONDITION_STATE_POS_NEGATIVE:
                case InteractionType.CONDITION_TIME_LESS_THAN:
                case InteractionType.CONDITION_TIME_MORE_THAN:
                case InteractionType.CONDITION_TRIGGER_ON_FURNI:
                case InteractionType.CONDITION_TRIGGER_ON_FURNI_NEGATIVE:
                case InteractionType.CONDITION_ACTOR_IN_GROUP:
                case InteractionType.CONDITION_NOT_IN_GROUP:
                case InteractionType.TRIGGER_BOT_REACHED_STF:
                case InteractionType.TRIGGER_BOT_REACHED_AVTR:
                case InteractionType.ACTION_BOT_CLOTHES:
                case InteractionType.ACTION_BOT_TELEPORT:
                case InteractionType.ACTION_BOT_FOLLOW_AVATAR:
                case InteractionType.ACTION_BOT_GIVE_HANDITEM:
                case InteractionType.ACTION_BOT_MOVE:
                case InteractionType.ACTION_USER_MOVE:
                case InteractionType.ACTION_BOT_TALK_TO_AVATAR:
                case InteractionType.ACTION_BOT_TALK:
                case InteractionType.CONDITION_HAS_HANDITEM:
                case InteractionType.ACTION_JOIN_TEAM:
                case InteractionType.ACTION_LEAVE_TEAM:
                case InteractionType.ACTION_GIVE_SCORE_TM:
                case InteractionType.CONDITION_ACTOR_IN_TEAM:
                case InteractionType.CONDITION_NOT_IN_TEAM:
                case InteractionType.CONDITION_NOT_USER_COUNT:
                case InteractionType.CONDITION_USER_COUNT_IN:
                    return new InteractorWired();
                case InteractionType.MANNEQUIN:
                    return new InteractorManiqui();
                case InteractionType.TONER:
                    return new InteractorChangeBackgrounds();
                case InteractionType.PUZZLEBOX:
                    return new InteractorPuzzleBox();
                case InteractionType.FLOORSWITCH1:
                    return new InteractorSwitch(item.GetBaseItem().Modes);
                case InteractionType.CRACKABLE:
                    return new InteractorCrackable(item.GetBaseItem().Modes);
                case InteractionType.TVYOUTUBE:
                    return new InteractorTvYoutube();
                case InteractionType.LOVELOCK:
                    return new InteractorLoveLock();
                case InteractionType.PHOTO:
                    return new InteractorIgnore();
                case InteractionType.BANZAITELE:
                    return new InteractorBanzaiTele();
                case InteractionType.HIGHSCORE:
                case InteractionType.HIGHSCOREPOINTS:
                    return new InteractorGenericSwitch(2);
                default:
                    return new InteractorGenericSwitch(item.GetBaseItem().Modes);
            }
        }
    }
}
