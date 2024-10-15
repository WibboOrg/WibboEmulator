namespace WibboEmulator.Communication.Packets;
using System.Diagnostics;
using WibboEmulator.Communication.Packets.Incoming;
using WibboEmulator.Communication.Packets.Incoming.Avatar;
using WibboEmulator.Communication.Packets.Incoming.Camera;
using WibboEmulator.Communication.Packets.Incoming.Campaign;
using WibboEmulator.Communication.Packets.Incoming.Catalog;
using WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Communication.Packets.Incoming.Guide;
using WibboEmulator.Communication.Packets.Incoming.Handshake;
using WibboEmulator.Communication.Packets.Incoming.Inventory.Achievements;
using WibboEmulator.Communication.Packets.Incoming.Inventory.AvatarEffects;
using WibboEmulator.Communication.Packets.Incoming.Inventory.Badges;
using WibboEmulator.Communication.Packets.Incoming.Inventory.Bots;
using WibboEmulator.Communication.Packets.Incoming.Inventory.Furni;
using WibboEmulator.Communication.Packets.Incoming.Inventory.Pets;
using WibboEmulator.Communication.Packets.Incoming.Inventory.Purse;
using WibboEmulator.Communication.Packets.Incoming.Inventory.Trading;
using WibboEmulator.Communication.Packets.Incoming.LandingView;
using WibboEmulator.Communication.Packets.Incoming.Marketplace;
using WibboEmulator.Communication.Packets.Incoming.Messenger;
using WibboEmulator.Communication.Packets.Incoming.Misc;
using WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Communication.Packets.Incoming.Navigator;
using WibboEmulator.Communication.Packets.Incoming.Quests;
using WibboEmulator.Communication.Packets.Incoming.RolePlay;
using WibboEmulator.Communication.Packets.Incoming.RolePlay.Troc;
using WibboEmulator.Communication.Packets.Incoming.Rooms.Action;
using WibboEmulator.Communication.Packets.Incoming.Rooms.AI.Bots;
using WibboEmulator.Communication.Packets.Incoming.Rooms.AI.Pets;
using WibboEmulator.Communication.Packets.Incoming.Rooms.AI.Pets.Horse;
using WibboEmulator.Communication.Packets.Incoming.Rooms.Avatar;
using WibboEmulator.Communication.Packets.Incoming.Rooms.Chat;
using WibboEmulator.Communication.Packets.Incoming.Rooms.Connection;
using WibboEmulator.Communication.Packets.Incoming.Rooms.Engine;
using WibboEmulator.Communication.Packets.Incoming.Rooms.FloorPlan;
using WibboEmulator.Communication.Packets.Incoming.Rooms.Furni;
using WibboEmulator.Communication.Packets.Incoming.Rooms.Furni.LoveLocks;
using WibboEmulator.Communication.Packets.Incoming.Rooms.Furni.Moodlight;
using WibboEmulator.Communication.Packets.Incoming.Rooms.Furni.Stickys;
using WibboEmulator.Communication.Packets.Incoming.Rooms.Furni.Wired;
using WibboEmulator.Communication.Packets.Incoming.Rooms.Nux;
using WibboEmulator.Communication.Packets.Incoming.Rooms.Polls;
using WibboEmulator.Communication.Packets.Incoming.Rooms.Settings;
using WibboEmulator.Communication.Packets.Incoming.Settings;
using WibboEmulator.Communication.Packets.Incoming.Televisions;
using WibboEmulator.Communication.Packets.Incoming.Users;
using WibboEmulator.Communication.Packets.Incoming.WibboTool;
using WibboEmulator.Core;
using WibboEmulator.Core.Settings;
using WibboEmulator.Games.GameClients;

public static class PacketManager
{
    private static readonly Dictionary<int, IPacketEvent> IncomingPackets = [];
    private static readonly List<int> HandshakePacketIds = [ClientPacketHeader.RELEASE_VERSION, ClientPacketHeader.SECURITY_TICKET, ClientPacketHeader.CLIENT_PONG];

    private static readonly TimeSpan MaximumRunTimeInSec = TimeSpan.FromSeconds(5);

    public static void Initialize()
    {
        UnregisterAll();

        RegisterHandshake();
        RegisterLandingView();
        RegisterCatalog();
        RegisterNavigator();
        RegisterMarketplace();
        RegisterRoomAction();
        RegisterQuests();
        RegisterRoomConnection();
        RegisterRoomChat();
        RegisterRoomEngine();
        RegisterFurni();
        RegisterUsers();
        RegisterSettings();
        RegisterMisc();
        RegisterInventory();
        RegisterPurse();
        RegisterRoomAvatar();
        RegisterAvatar();
        RegisterMessenger();
        RegisterGroups();
        RegisterRoomSettings();
        RegisterPets();
        RegisterBots();
        RegisterFloorPlanEditor();
        RegisterModeration();
        RegisterGuide();
        RegisterNux();
        RegisterCamera();
        RegisterCampaign();
        RegisterCustom();

        Console.WriteLine("Logged " + IncomingPackets.Count + " packet handler(s)!");
    }

    public static void TryExecutePacket(GameClient Session, ClientPacket packet)
    {
        var timeStarted = DateTime.Now;

        if (Session.User == null && !HandshakePacketIds.Contains(packet.Id))
        {
            if (Debugger.IsAttached)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[" + packet.Id + "] unauthorized packet Id");
                Console.ResetColor();
            }
            return;
        }

        if (!IncomingPackets.TryGetValue(packet.Id, out var pak))
        {
            if (Debugger.IsAttached)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(packet.ToString());
                Console.ResetColor();
            }
            return;
        }

        if (Debugger.IsAttached)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(packet.ToString());
            Console.ResetColor();
        }

        if (Session.PacketTimeout(packet.Id, pak.Delay))
        {
            if (Debugger.IsAttached)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("[" + packet.Id + "] Spam detected");
                Console.ResetColor();
            }

            if (SettingsManager.GetData<bool>("packet.log.lantency.enable"))
            {
                ExceptionLogger.LogPacketException(packet.ToString(), string.Format("Spam detected in {0}: {1}ms", Session.User?.Username ?? Session.Connection.Ip, pak.Delay));
            }
            return;
        }

        pak.Parse(Session, packet);

        var timeExecution = DateTime.Now - timeStarted;
        if (timeExecution > MaximumRunTimeInSec)
        {
            ExceptionLogger.LogPacketException(packet.ToString(), string.Format("High latency in {0}: {1}ms", Session.User?.Username ?? Session.Connection.Ip, timeExecution.TotalMilliseconds));
        }
    }

    /*private async Task ExecutePacketAsync(GameClient Session, ClientPacket packet, IPacketEvent pak)
    {
        if ( _cancellationTokenSource.IsCancellationRequested)
        {
            return;
        }

        var task = new Task(() => pak.Parse(Session, packet));
        task.Start();

        await task.WaitAsync( _maximumRunTimeInSec,  _cancellationTokenSource.Token).ContinueWith(t =>
        {
            if (t.IsFaulted && t.Exception != null)
            {
                foreach (var e in t.Exception.Flatten().InnerExceptions)
                {
                    var messageError = string.Format("Error handling packet {0} for session {1} @ User Name {2}: {3}", packet.Id, Session.ConnectionID, Session.GetUser()?.Username ?? string.Empty, e.Message);
                    ExceptionLogger.LogPacketException(packet.Id.ToString(), messageError);
                }
            }
        });
    }*/

    public static void UnregisterAll() => IncomingPackets.Clear();

    private static void RegisterCustom()
    {
        IncomingPackets.Add(ClientPacketHeader.SEND_ALERT, new SendHotelAlertEvent());
        IncomingPackets.Add(ClientPacketHeader.EDIT_TV, new EditTvYoutubeEvent());
        IncomingPackets.Add(ClientPacketHeader.MOVE_AVATAR_KEYBOARD, new MoveAvatarKeyboardEvent());
        IncomingPackets.Add(ClientPacketHeader.RP_BUY_ITEMS, new RpBuyItemsEvent());
        IncomingPackets.Add(ClientPacketHeader.RP_USE_ITEMS, new RpUseItemsEvent());
        IncomingPackets.Add(ClientPacketHeader.RP_TROC_ADD_ITEM, new RpTrocAddItemEvent());
        IncomingPackets.Add(ClientPacketHeader.RP_TROC_REMOVE_ITEM, new RpTrocRemoveItemEvent());
        IncomingPackets.Add(ClientPacketHeader.RP_TROC_ACCEPTE, new RpTrocAccepteEvent());
        IncomingPackets.Add(ClientPacketHeader.RP_TROC_CONFIRME, new RpTrocConfirmeEvent());
        IncomingPackets.Add(ClientPacketHeader.RP_TROC_STOP, new RpTrocStopEvent());
        IncomingPackets.Add(ClientPacketHeader.BOT_CHOOSE, new RpBotChooseEvent());
    }

    private static void RegisterCampaign()
    {
        IncomingPackets.Add(ClientPacketHeader.OPEN_CAMPAIGN_CALENDAR_DOOR, new OpenCampaignCalendarDoorEvent());
        IncomingPackets.Add(ClientPacketHeader.OPEN_CAMPAIGN_CALENDAR_DOOR_STAFF, new OpenCampaignCalendarDoorAsStaffEvent());
    }

    private static void RegisterHandshake()
    {
        IncomingPackets.Add(ClientPacketHeader.RELEASE_VERSION, new GetClientVersionEvent());
        IncomingPackets.Add(ClientPacketHeader.SECURITY_TICKET, new SSOTicketEvent());
        IncomingPackets.Add(ClientPacketHeader.USER_INFO, new InfoRetrieveEvent());
        IncomingPackets.Add(ClientPacketHeader.CLIENT_PONG, new PingEvent());
    }

    private static void RegisterLandingView()
    {
        IncomingPackets.Add(ClientPacketHeader.GET_CURRENT_TIMING_CODE, new RefreshCampaignEvent());
        IncomingPackets.Add(ClientPacketHeader.DESKTOP_NEWS, new GetPromoArticlesEvent());
        IncomingPackets.Add(ClientPacketHeader.GET_COMMUNITY_GOAL_HALL_OF_FAME, new GetCommunityGoalHallOfFameEvent());
    }

    private static void RegisterNux() => IncomingPackets.Add(ClientPacketHeader.USER_NUX_EVENT, new RoomNuxAlertEvent());

    private static void RegisterCatalog()
    {
        IncomingPackets.Add(ClientPacketHeader.GET_CATALOG_INDEX, new GetCatalogIndexEvent());
        IncomingPackets.Add(ClientPacketHeader.GET_CATALOG_PAGE, new GetCatalogPageEvent());
        IncomingPackets.Add(ClientPacketHeader.GET_PRODUCT_OFFER, new GetCatalogOfferEvent());
        IncomingPackets.Add(ClientPacketHeader.CATALOG_PURCHASE, new PurchaseFromCatalogEvent());
        IncomingPackets.Add(ClientPacketHeader.CATALOG_PURCHASE_GIFT, new PurchaseFromCatalogAsGiftEvent());
        IncomingPackets.Add(ClientPacketHeader.GET_GIFT_WRAPPING_CONFIG, new GetGiftWrappingConfigurationEvent());
        IncomingPackets.Add(ClientPacketHeader.APPROVE_NAME, new CheckPetNameEvent());
        IncomingPackets.Add(ClientPacketHeader.CATALOG_REDEEM_VOUCHER, new RedeemVoucherEvent());
        IncomingPackets.Add(ClientPacketHeader.CATALOG_REQUESET_PET_BREEDS, new GetSellablePetBreedsEvent());
        IncomingPackets.Add(ClientPacketHeader.GROUP_MEMBERSHIPS, new GetGroupFurniConfigEvent());
        IncomingPackets.Add(ClientPacketHeader.MARKETPLACE_CONFIG, new GetMarketplaceConfigurationEvent());
        IncomingPackets.Add(ClientPacketHeader.GET_CLUB_OFFERS, new GetClubOffersEvent());
        IncomingPackets.Add(ClientPacketHeader.CATALOG_SELECT_VIP_GIFT, new SelectVipGiftEvent());
    }

    private static void RegisterCamera()
    {
        IncomingPackets.Add(ClientPacketHeader.REQUEST_CAMERA_CONFIGURATION, new RequestCameraConfigurationEvent());
        IncomingPackets.Add(ClientPacketHeader.RENDER_ROOM_THUMBNAIL, new RenderRoomThumbnailEvent());
        IncomingPackets.Add(ClientPacketHeader.RENDER_ROOM, new RenderRoomEvent());
        IncomingPackets.Add(ClientPacketHeader.PURCHASE_PHOTO, new PurchasePhotoEvent());
        IncomingPackets.Add(ClientPacketHeader.PUBLISH_PHOTO, new PublishPhotoEvent());
        IncomingPackets.Add(ClientPacketHeader.PHOTO_COMPETITION, new PhotoCompetitionEvent());
    }

    private static void RegisterMarketplace()
    {
        IncomingPackets.Add(ClientPacketHeader.MARKETPLACE_REQUEST_OFFERS, new GetOffersEvent());
        IncomingPackets.Add(ClientPacketHeader.MARKETPLACE_REQUEST_OWN_ITEMS, new GetOwnOffersEvent());
        IncomingPackets.Add(ClientPacketHeader.REQUEST_SELL_ITEM, new GetMarketplaceCanMakeOfferEvent());
        IncomingPackets.Add(ClientPacketHeader.REQUEST_MARKETPLACE_ITEM_STATS, new GetMarketplaceItemStatsEvent());
        IncomingPackets.Add(ClientPacketHeader.MARKETPLACE_SELL_ITEM, new MakeOfferEvent());
        IncomingPackets.Add(ClientPacketHeader.MARKETPLACE_TAKE_BACK_ITEM, new CancelOfferEvent());
        IncomingPackets.Add(ClientPacketHeader.MARKETPLACE_BUY_OFFER, new BuyOfferEvent());
        IncomingPackets.Add(ClientPacketHeader.MARKETPLACE_REDEEM_CREDITS, new RedeemOfferCreditsEvent());
    }

    private static void RegisterNavigator()
    {
        IncomingPackets.Add(ClientPacketHeader.NAVIGATOR_INIT, new InitializeNewNavigatorEvent());
        IncomingPackets.Add(ClientPacketHeader.NAVIGATOR_SEARCH, new NavigatorSearchEvent());
        IncomingPackets.Add(ClientPacketHeader.NAVIGATOR_SETTINGS, new NavigatorSettingsEvent());
        IncomingPackets.Add(ClientPacketHeader.NAVIGATOR_CATEGORIES, new GetUserFlatCatsEvent());
    }

    private static void RegisterRoomConnection()
    {
        IncomingPackets.Add(ClientPacketHeader.DESKTOP_VIEW, new GoToHotelViewEvent());
        IncomingPackets.Add(ClientPacketHeader.ROOM_ENTER, new OpenFlatConnectionEvent());
        IncomingPackets.Add(ClientPacketHeader.GO_TO_FLAT, new GoToFlatEvent());
    }

    private static void RegisterRoomSettings()
    {
        IncomingPackets.Add(ClientPacketHeader.ROOM_SETTINGS, new GetRoomSettingsEvent());
        IncomingPackets.Add(ClientPacketHeader.ROOM_SETTINGS_SAVE, new SaveRoomSettingsEvent());
        IncomingPackets.Add(ClientPacketHeader.ROOM_DELETE, new DeleteRoomEvent());
        IncomingPackets.Add(ClientPacketHeader.ROOM_MUTE, new ToggleMuteToolEvent());


        IncomingPackets.Add(ClientPacketHeader.ROOM_RIGHTS_LIST, new GetRoomRightsEvent());
        IncomingPackets.Add(ClientPacketHeader.ROOM_BAN_LIST, new GetRoomBannedUsersEvent());
        IncomingPackets.Add(ClientPacketHeader.ROOM_BAN_REMOVE, new UnbanUserFromRoomEvent());

    }

    private static void RegisterFloorPlanEditor()
    {
        IncomingPackets.Add(ClientPacketHeader.ROOM_MODEL_SAVE, new SaveFloorPlanModelEvent());
        IncomingPackets.Add(ClientPacketHeader.GET_ROOM_ENTRY_TILE, new InitializeFloorPlanSessionEvent());
        IncomingPackets.Add(ClientPacketHeader.GET_OCCUPIED_TILES, new GetOccupiedTilesEvent());
    }

    private static void RegisterAvatar()
    {
        IncomingPackets.Add(ClientPacketHeader.GET_WARDROBE, new GetWardrobeEvent());
        IncomingPackets.Add(ClientPacketHeader.SAVE_WARDROBE_OUTFIT, new SaveWardrobeOutfitEvent());
    }

    private static void RegisterRoomAction()
    {
        IncomingPackets.Add(ClientPacketHeader.ROOM_DOORBELL, new LetUserInEvent());
        IncomingPackets.Add(ClientPacketHeader.ROOM_BAN_GIVE, new BanUserEvent());
        IncomingPackets.Add(ClientPacketHeader.ROOM_KICK, new KickUserEvent());
        IncomingPackets.Add(ClientPacketHeader.ROOM_RIGHTS_GIVE, new AssignRightsEvent());
        IncomingPackets.Add(ClientPacketHeader.ROOM_RIGHTS_REMOVE, new RemoveRightsEvent());
        IncomingPackets.Add(ClientPacketHeader.ROOM_RIGHTS_REMOVE_ALL, new RemoveAllRightsEvent());
        IncomingPackets.Add(ClientPacketHeader.ROOM_MUTE_USER, new MuteUserEvent());
        IncomingPackets.Add(ClientPacketHeader.UNIT_GIVE_HANDITEM, new GiveHandItemEvent());
        IncomingPackets.Add(ClientPacketHeader.ROOM_RIGHTS_REMOVE_OWN, new RemoveMyRightsEvent());
    }

    private static void RegisterRoomEngine()
    {
        IncomingPackets.Add(ClientPacketHeader.ROOM_MODEL, new GetRoomEntryDataEvent());
        IncomingPackets.Add(ClientPacketHeader.FURNITURE_ALIASES, new GetFurnitureAliasesMessageEvent());
        IncomingPackets.Add(ClientPacketHeader.UNIT_WALK, new MoveAvatarEvent());
        IncomingPackets.Add(ClientPacketHeader.FURNITURE_FLOOR_UPDATE, new MoveObjectEvent());
        IncomingPackets.Add(ClientPacketHeader.FURNITURE_PICKUP, new PickupObjectEvent());
        IncomingPackets.Add(ClientPacketHeader.FURNITURE_PICKUP_ALL, new PickupObjectAllEvent());
        IncomingPackets.Add(ClientPacketHeader.FURNITURE_WALL_UPDATE, new MoveWallItemEvent());
        IncomingPackets.Add(ClientPacketHeader.ITEM_PAINT, new ApplyDecorationEvent());
        IncomingPackets.Add(ClientPacketHeader.FURNITURE_PLACE, new PlaceObjectEvent());
        IncomingPackets.Add(ClientPacketHeader.FURNITURE_MULTISTATE, new UseFurnitureEvent());
        IncomingPackets.Add(ClientPacketHeader.FURNITURE_WALL_MULTISTATE, new UseFurnitureEvent());
        IncomingPackets.Add(ClientPacketHeader.FURNITURE_RANDOMSTATE, new UseFurnitureEvent());
        IncomingPackets.Add(ClientPacketHeader.POLL_ANSWER, new AnswerPollEvent());
        IncomingPackets.Add(ClientPacketHeader.ROOM_FAVORITE, new AddFavouriteRoomEvent());
        IncomingPackets.Add(ClientPacketHeader.ROOM_FAVORITE_REMOVE, new RemoveFavouriteRoomEvent());
        IncomingPackets.Add(ClientPacketHeader.USER_HOME_ROOM, new NavigatorHomeRoomEvent());
        IncomingPackets.Add(ClientPacketHeader.CAN_CREATE_ROOM_MESSAGE_EVENT, new CanCreateRoomEvent());
        IncomingPackets.Add(ClientPacketHeader.ROOM_CREATE, new CreateFlatEvent());
        IncomingPackets.Add(ClientPacketHeader.ROOM_INFO, new GetGuestRoomEvent());
    }

    private static void RegisterRoomChat()
    {
        IncomingPackets.Add(ClientPacketHeader.UNIT_CHAT, new ChatEvent());
        IncomingPackets.Add(ClientPacketHeader.UNIT_CHAT_SHOUT, new ChatEvent(true));
        IncomingPackets.Add(ClientPacketHeader.UNIT_CHAT_WHISPER, new WhisperEvent());
        IncomingPackets.Add(ClientPacketHeader.UNIT_CHAT_AUDIO, new ChatAudioEvent());
        IncomingPackets.Add(ClientPacketHeader.UNIT_TYPING, new StartTypingEvent());
        IncomingPackets.Add(ClientPacketHeader.UNIT_TYPING_STOP, new CancelTypingEvent());
    }

    private static void RegisterInventory()
    {
        IncomingPackets.Add(ClientPacketHeader.TRADE, new InitTradeEvent());
        IncomingPackets.Add(ClientPacketHeader.TRADE_ITEM, new TradingOfferItemEvent());
        IncomingPackets.Add(ClientPacketHeader.TRADE_ITEMS, new TradeOfferMultipleItemsEvent());

        IncomingPackets.Add(ClientPacketHeader.TRADE_ITEM_REMOVE, new TradingRemoveItemEvent());
        IncomingPackets.Add(ClientPacketHeader.TRADE_ACCEPT, new TradingAcceptEvent());
        IncomingPackets.Add(ClientPacketHeader.TRADE_CLOSE, new TradingCancelEvent());
        IncomingPackets.Add(ClientPacketHeader.TRADE_CONFIRM, new TradingConfirmEvent());
        IncomingPackets.Add(ClientPacketHeader.TRADE_UNACCEPT, new TradingModifyEvent());
        IncomingPackets.Add(ClientPacketHeader.TRADE_CANCEL, new TradingCancelConfirmEvent());

        IncomingPackets.Add(ClientPacketHeader.USER_FURNITURE, new RequestFurniInventoryEvent());
        IncomingPackets.Add(ClientPacketHeader.USER_BADGES, new GetBadgesEvent());
        IncomingPackets.Add(ClientPacketHeader.ACHIEVEMENT_LIST, new GetAchievementsEvent());
        IncomingPackets.Add(ClientPacketHeader.USER_BADGES_CURRENT_UPDATE, new SetActivatedBadgesEvent());
        IncomingPackets.Add(ClientPacketHeader.USER_BOTS, new GetBotInventoryEvent());
        IncomingPackets.Add(ClientPacketHeader.USER_PETS, new GetPetInventoryEvent());
        IncomingPackets.Add(ClientPacketHeader.USER_EFFECT_ACTIVATE, new AvatarEffectActivatedEvent());
        IncomingPackets.Add(ClientPacketHeader.USER_EFFECT_ENABLE, new AvatarEffectSelectedEvent());

        IncomingPackets.Add(ClientPacketHeader.DELETE_BADGE_INVENTORY, new DeleteBadgeInventoryEvent());
        IncomingPackets.Add(ClientPacketHeader.DELETE_FURNITURE_TYPE_INVENTORY, new DeleteFurniTypeInventoryEvent());
    }

    private static void RegisterPurse() => IncomingPackets.Add(ClientPacketHeader.USER_CURRENCY, new GetCreditsInfoEvent());

    private static void RegisterMessenger()
    {
        IncomingPackets.Add(ClientPacketHeader.MESSENGER_INIT, new MessengerInitEvent());
        IncomingPackets.Add(ClientPacketHeader.GET_FRIEND_REQUESTS, new GetBuddyRequestsEvent());
        IncomingPackets.Add(ClientPacketHeader.FOLLOW_FRIEND, new FollowFriendEvent());
        IncomingPackets.Add(ClientPacketHeader.FIND_NEW_FRIENDS, new FindNewFriendsEvent());

        IncomingPackets.Add(ClientPacketHeader.REMOVE_FRIEND, new RemoveBuddyEvent());
        IncomingPackets.Add(ClientPacketHeader.REQUEST_FRIEND, new RequestBuddyEvent());
        IncomingPackets.Add(ClientPacketHeader.MESSENGER_CHAT, new SendMsgEvent());
        IncomingPackets.Add(ClientPacketHeader.SEND_ROOM_INVITE, new SendRoomInviteEvent());
        IncomingPackets.Add(ClientPacketHeader.HABBO_SEARCH, new UserSearchEvent());
        IncomingPackets.Add(ClientPacketHeader.ACCEPT_FRIEND, new AcceptBuddyEvent());
        IncomingPackets.Add(ClientPacketHeader.DECLINE_FRIEND, new DeclineBuddyEvent());
    }

    private static void RegisterGroups()
    {
        IncomingPackets.Add(ClientPacketHeader.GROUP_REQUEST, new JoinGroupEvent());
        IncomingPackets.Add(ClientPacketHeader.GROUP_UNFAVORITE, new RemoveGroupFavouriteEvent());
        IncomingPackets.Add(ClientPacketHeader.GROUP_FAVORITE, new SetGroupFavouriteEvent());
        IncomingPackets.Add(ClientPacketHeader.GROUP_INFO, new GetGroupInfoEvent());
        IncomingPackets.Add(ClientPacketHeader.GROUP_MEMBERS, new GetGroupMembersEvent());
        IncomingPackets.Add(ClientPacketHeader.GROUP_CREATE_OPTIONS, new GetGroupCreationWindowEvent());
        IncomingPackets.Add(ClientPacketHeader.GROUP_PARTS, new GetBadgeEditorPartsEvent());
        IncomingPackets.Add(ClientPacketHeader.GROUP_BUY, new PurchaseGroupEvent());
        IncomingPackets.Add(ClientPacketHeader.GROUP_SAVE_INFORMATION, new UpdateGroupIdentityEvent());
        IncomingPackets.Add(ClientPacketHeader.GROUP_SAVE_BADGE, new UpdateGroupBadgeEvent());
        IncomingPackets.Add(ClientPacketHeader.GROUP_SAVE_COLORS, new UpdateGroupColoursEvent());
        IncomingPackets.Add(ClientPacketHeader.GROUP_SAVE_PREFERENCES, new UpdateGroupSettingsEvent());
        IncomingPackets.Add(ClientPacketHeader.GROUP_SETTINGS, new ManageGroupEvent());
        IncomingPackets.Add(ClientPacketHeader.GROUP_ADMIN_ADD, new GiveAdminRightsEvent());
        IncomingPackets.Add(ClientPacketHeader.GROUP_ADMIN_REMOVE, new TakeAdminRightsEvent());
        IncomingPackets.Add(ClientPacketHeader.GROUP_MEMBER_REMOVE_CONFIRM, new RemoveGroupMemberEvent());
        IncomingPackets.Add(ClientPacketHeader.GROUP_MEMBER_REMOVE, new RemoveGroupMemberEvent());
        IncomingPackets.Add(ClientPacketHeader.GROUP_REQUEST_ACCEPT, new AcceptGroupMembershipEvent());
        IncomingPackets.Add(ClientPacketHeader.GROUP_REQUEST_DECLINE, new DeclineGroupMembershipEvent());
        IncomingPackets.Add(ClientPacketHeader.GROUP_DELETE, new DeleteGroupEvent());
    }

    private static void RegisterPets()
    {
        IncomingPackets.Add(ClientPacketHeader.PET_RESPECT, new RespectPetEvent());
        IncomingPackets.Add(ClientPacketHeader.PET_INFO, new GetPetInformationEvent());
        IncomingPackets.Add(ClientPacketHeader.PET_PICKUP, new PickUpPetEvent());
        IncomingPackets.Add(ClientPacketHeader.PET_PLACE, new PlacePetEvent());
        IncomingPackets.Add(ClientPacketHeader.PET_RIDE, new RideHorseEvent());
        IncomingPackets.Add(ClientPacketHeader.USE_PET_PRODUCT, new ApplyHorseEffectEvent());
        IncomingPackets.Add(ClientPacketHeader.REMOVE_PET_SADDLE, new RemoveSaddleFromHorseEvent());
        IncomingPackets.Add(ClientPacketHeader.TOGGLE_PET_RIDING, new ModifyWhoCanRideHorseEvent());
        IncomingPackets.Add(ClientPacketHeader.GET_PET_TRAINING_PANEL_MESSAGE_EVENT, new GetPetTrainingPanelEvent());
        IncomingPackets.Add(ClientPacketHeader.PET_MOVE, new MoveMonsterPlanteEvent());
    }

    private static void RegisterQuests()
    {
        IncomingPackets.Add(ClientPacketHeader.GET_QUESTS, new GetQuestListEvent());
        IncomingPackets.Add(ClientPacketHeader.ACCEPT_QUEST, new StartQuestEvent());
        IncomingPackets.Add(ClientPacketHeader.REJECT_QUEST, new CancelQuestEvent());
        IncomingPackets.Add(ClientPacketHeader.OPEN_QUEST_TRACKER, new GetCurrentQuestEvent());
    }

    private static void RegisterFurni()
    {
        IncomingPackets.Add(ClientPacketHeader.ITEM_STACK_HELPER, new UpdateMagicTileEvent());
        IncomingPackets.Add(ClientPacketHeader.WIRED_TRIGGER_SAVE, new UpdateTriggerEvent());
        IncomingPackets.Add(ClientPacketHeader.WIRED_ACTION_SAVE, new UpdateActionEvent());
        IncomingPackets.Add(ClientPacketHeader.WIRED_CONDITION_SAVE, new UpdateConditionEvent());
        IncomingPackets.Add(ClientPacketHeader.SET_OBJECT_DATA, new SaveBrandingItemEvent());
        IncomingPackets.Add(ClientPacketHeader.ROOM_TONER_APPLY, new SetTonerEvent());
        IncomingPackets.Add(ClientPacketHeader.ITEM_DICE_CLOSE, new UseFurnitureEvent());
        IncomingPackets.Add(ClientPacketHeader.ITEM_DICE_CLICK, new UseFurnitureEvent());
        IncomingPackets.Add(ClientPacketHeader.MANNEQUIN_SAVE_NAME, new SetMannequinNameEvent());
        IncomingPackets.Add(ClientPacketHeader.MANNEQUIN_SAVE_LOOK, new SetMannequinFigureEvent());
        IncomingPackets.Add(ClientPacketHeader.ITEM_EXCHANGE_REDEEM, new CreditFurniRedeemEvent());
        IncomingPackets.Add(ClientPacketHeader.GET_ITEM_DATA, new GetStickyNoteEvent());
        IncomingPackets.Add(ClientPacketHeader.FURNITURE_POSTIT_PLACE, new AddStickyNoteEvent());
        IncomingPackets.Add(ClientPacketHeader.SET_ITEM_DATA, new UpdateStickyNoteEvent());
        IncomingPackets.Add(ClientPacketHeader.REMOVE_WALL_ITEM, new DeleteStickyNoteEvent());
        IncomingPackets.Add(ClientPacketHeader.ITEM_DIMMER_SETTINGS, new GetMoodlightConfigEvent());
        IncomingPackets.Add(ClientPacketHeader.ITEM_DIMMER_SAVE, new MoodlightUpdateEvent());
        IncomingPackets.Add(ClientPacketHeader.ITEM_DIMMER_TOGGLE, new ToggleMoodlightEvent());
        IncomingPackets.Add(ClientPacketHeader.ONE_WAY_DOOR_CLICK, new UseFurnitureEvent());
        IncomingPackets.Add(ClientPacketHeader.ITEM_COLOR_WHEEL_CLICK, new UseFurnitureEvent());
        IncomingPackets.Add(ClientPacketHeader.FOOTBALL_GATE_SAVE_LOOK_EVENT, new ChangeFootGate());
        IncomingPackets.Add(ClientPacketHeader.PRESENT_OPEN_PRESENT, new OpenGiftEvent());
        IncomingPackets.Add(ClientPacketHeader.FURNITURE_GROUP_INFO, new GetGroupFurniSettingsEvent());
        IncomingPackets.Add(ClientPacketHeader.FRIEND_FURNI_CONFIRM_LOCK, new ConfirmLoveLockEvent());
    }

    private static void RegisterUsers()
    {
        IncomingPackets.Add(ClientPacketHeader.USER_SUBSCRIPTION, new ScrGetUserInfoMessageEvent());
        IncomingPackets.Add(ClientPacketHeader.USER_RESPECT, new RespectUserEvent());
        IncomingPackets.Add(ClientPacketHeader.USER_FIGURE, new UpdateFigureDataEvent());
        IncomingPackets.Add(ClientPacketHeader.USER_PROFILE, new OpenPlayerProfileEvent());
        IncomingPackets.Add(ClientPacketHeader.USER_BADGES_CURRENT, new GetSelectedBadgesEvent());
        IncomingPackets.Add(ClientPacketHeader.MESSENGER_RELATIONSHIPS, new GetRelationshipsEvent());
        IncomingPackets.Add(ClientPacketHeader.SET_RELATIONSHIP_STATUS, new SetRelationshipEvent());
        IncomingPackets.Add(ClientPacketHeader.CHECK_USERNAME, new CheckValidNameEvent());
        IncomingPackets.Add(ClientPacketHeader.CHANGE_USERNAME, new ChangeNameEvent());
        IncomingPackets.Add(ClientPacketHeader.GROUP_BADGES, new GetUserGroupBadgesEvent());
        IncomingPackets.Add(ClientPacketHeader.USER_BANNER, new UserBannerEvent());
        IncomingPackets.Add(ClientPacketHeader.USER_BANNER_SELECT, new UserBannerSelectEvent());
    }

    private static void RegisterSettings()
    {
        IncomingPackets.Add(ClientPacketHeader.USER_SETTINGS_OLD_CHAT, new UserSettingsOldChatEvent());
        IncomingPackets.Add(ClientPacketHeader.USER_SETTINGS_INVITES, new UserSettingsRoomInvitesEvent());
        IncomingPackets.Add(ClientPacketHeader.USER_SETTINGS_CAMERA, new UserSettingsCameraFollowEvent());
        IncomingPackets.Add(ClientPacketHeader.USER_SETTINGS_VOLUME, new UserSettingsSoundEvent());
    }

    private static void RegisterMisc()
    {
        IncomingPackets.Add(ClientPacketHeader.CLIENT_LATENCY, new LatencyTestEvent());
        IncomingPackets.Add(ClientPacketHeader.CLIENT_TOOLBAR_TOGGLE, new SetFriendBarStateEvent());
    }

    private static void RegisterRoomAvatar()
    {
        IncomingPackets.Add(ClientPacketHeader.UNIT_ACTION, new ActionEvent());
        IncomingPackets.Add(ClientPacketHeader.UNIT_SIGN, new ApplySignEvent());
        IncomingPackets.Add(ClientPacketHeader.UNIT_DANCE, new DanceEvent());
        IncomingPackets.Add(ClientPacketHeader.UNIT_POSTURE, new SitEvent());
        IncomingPackets.Add(ClientPacketHeader.USER_MOTTO, new ChangeMottoEvent());
        IncomingPackets.Add(ClientPacketHeader.UNIT_LOOK, new LookToEvent());
        IncomingPackets.Add(ClientPacketHeader.UNIT_DROP_HAND_ITEM, new DropHandItemEvent());
        IncomingPackets.Add(ClientPacketHeader.ROOM_LIKE, new GiveRoomScoreEvent());
        IncomingPackets.Add(ClientPacketHeader.USER_IGNORE, new IgnoreUserEvent());
        IncomingPackets.Add(ClientPacketHeader.USER_UNIGNORE, new UnIgnoreUserEvent());
        IncomingPackets.Add(ClientPacketHeader.UNIT_CHAT_WHISPER_GROUP, new WhiperGroupEvent());
    }

    private static void RegisterBots()
    {
        IncomingPackets.Add(ClientPacketHeader.BOT_PLACE, new PlaceBotEvent());
        IncomingPackets.Add(ClientPacketHeader.BOT_PICKUP, new PickUpBotEvent());
        IncomingPackets.Add(ClientPacketHeader.BOT_CONFIGURATION, new OpenBotActionEvent());
        IncomingPackets.Add(ClientPacketHeader.BOT_SKILL_SAVE, new SaveBotActionEvent());
    }

    private static void RegisterModeration()
    {
        IncomingPackets.Add(ClientPacketHeader.GET_PENDING_CALLS_FOR_HELP, new OpenHelpToolEvent());
        IncomingPackets.Add(ClientPacketHeader.MODTOOL_REQUEST_ROOM_INFO, new GetModeratorRoomInfoEvent());
        IncomingPackets.Add(ClientPacketHeader.MOD_TOOL_USER_INFO, new GetModeratorUserInfoEvent());
        IncomingPackets.Add(ClientPacketHeader.MODTOOL_REQUEST_USER_ROOMS, new GetModeratorUserRoomVisitsEvent());
        IncomingPackets.Add(ClientPacketHeader.MODTOOL_CHANGE_ROOM_SETTINGS, new ModerateRoomEvent());
        IncomingPackets.Add(ClientPacketHeader.MODTOOL_ROOM_ALERT, new ModeratorActionEvent());
        IncomingPackets.Add(ClientPacketHeader.CALL_FOR_HELP, new SubmitNewTicketEvent());
        IncomingPackets.Add(ClientPacketHeader.MODTOOL_REQUEST_ROOM_CHATLOG, new GetModeratorRoomChatlogEvent());
        IncomingPackets.Add(ClientPacketHeader.MODTOOL_REQUEST_USER_CHATLOG, new GetModeratorUserChatlogEvent());

        IncomingPackets.Add(ClientPacketHeader.PICK_ISSUES, new PickTicketEvent());
        IncomingPackets.Add(ClientPacketHeader.RELEASE_ISSUES, new ReleaseTicketEvent());
        IncomingPackets.Add(ClientPacketHeader.CLOSE_ISSUES, new CloseTicketEvent());

        IncomingPackets.Add(ClientPacketHeader.MODTOOL_SANCTION_MUTE, new ModerationMuteEvent());
        IncomingPackets.Add(ClientPacketHeader.MODTOOL_SANCTION_KICK, new ModerationMuteEvent());
        IncomingPackets.Add(ClientPacketHeader.MODTOOL_SANCTION_BAN, new ModerationBanEvent());
        IncomingPackets.Add(ClientPacketHeader.MODTOOL_ALERTEVENT, new ModerationMsgEvent());
        IncomingPackets.Add(ClientPacketHeader.MODTOOL_SANCTION_ALERT, new ModerationMsgEvent());
    }

    private static void RegisterGuide()
    {
        IncomingPackets.Add(ClientPacketHeader.GUIDE_SESSION_ON_DUTY_UPDATE, new GetHelperToolConfigurationEvent());
        IncomingPackets.Add(ClientPacketHeader.GUIDE_SESSION_GUIDE_DECIDES, new OnGuideSessionDetachedEvent());
        IncomingPackets.Add(ClientPacketHeader.GUIDE_SESSION_CREATE, new OnGuideEvent());
        IncomingPackets.Add(ClientPacketHeader.GUIDE_SESSION_FEEDBACK, new RecomendHelpersEvent());
        IncomingPackets.Add(ClientPacketHeader.GUIDE_SESSION_MESSAGE, new GuideToolMessageNewEvent());
        IncomingPackets.Add(ClientPacketHeader.GUIDE_SESSION_INVITE_REQUESTER, new GuideInviteToRoomEvent());
        IncomingPackets.Add(ClientPacketHeader.GUIDE_SESSION_GET_REQUESTER_ROOM, new VisitRoomGuidesEvent());
        IncomingPackets.Add(ClientPacketHeader.GUIDE_SESSION_RESOLVED, new GuideEndSessionEvent());
        IncomingPackets.Add(ClientPacketHeader.GUIDE_SESSION_REQUESTER_CANCELS, new CancellInviteGuideEvent());
    }
}
