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
using WibboEmulator.Games.GameClients;

public sealed class PacketManager
{
    private readonly Dictionary<int, IPacketEvent> _incomingPackets;
    private readonly List<int> _handshakePacketIds = new() { ClientPacketHeader.RELEASE_VERSION, ClientPacketHeader.SECURITY_TICKET, ClientPacketHeader.CLIENT_PONG };

    private readonly TimeSpan _maximumRunTimeInSec = TimeSpan.FromSeconds(5);

    public PacketManager() => this._incomingPackets = new Dictionary<int, IPacketEvent>();

    public void Init()
    {
        this.UnregisterAll();

        this.RegisterHandshake();
        this.RegisterLandingView();
        this.RegisterCatalog();
        this.RegisterNavigator();
        this.RegisterMarketplace();
        this.RegisterRoomAction();
        this.RegisterQuests();
        this.RegisterRoomConnection();
        this.RegisterRoomChat();
        this.RegisterRoomEngine();
        this.RegisterFurni();
        this.RegisterUsers();
        this.RegisterSettings();
        this.RegisterMisc();
        this.RegisterInventory();
        this.RegisterPurse();
        this.RegisterRoomAvatar();
        this.RegisterAvatar();
        this.RegisterMessenger();
        this.RegisterGroups();
        this.RegisterRoomSettings();
        this.RegisterPets();
        this.RegisterBots();
        this.FloorPlanEditor();
        this.RegisterModeration();
        this.RegisterGuide();
        this.RegisterNux();
        this.RegisterCamera();
        this.RegisterCampaign();
        this.RegisterCustom();

        Console.WriteLine("Logged " + this._incomingPackets.Count + " packet handler(s)!");
    }

    public void TryExecutePacket(GameClient session, ClientPacket packet)
    {
        var timeStarted = DateTime.Now;

        if (session.User == null && !this._handshakePacketIds.Contains(packet.Id))
        {
            if (Debugger.IsAttached)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[" + packet.Id + "] unauthorized packet Id");
                Console.ResetColor();
            }
            return;
        }

        if (!this._incomingPackets.TryGetValue(packet.Id, out var pak))
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

        if (session.PacketTimeout(packet.Id, pak.Delay))
        {
            if (Debugger.IsAttached)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("[" + packet.Id + "] Spam detected");
                Console.ResetColor();
            }

            if (WibboEnvironment.GetSettings().GetData<bool>("packet.log.lantency.enable"))
            {
                ExceptionLogger.LogPacketException(packet.ToString(), string.Format("Spam detected in {0}: {1}ms", session.User?.Username ?? session.Connection.GetIp(), pak.Delay));
            }
            return;
        }

        pak.Parse(session, packet);

        var timeExecution = DateTime.Now - timeStarted;
        if (timeExecution > this._maximumRunTimeInSec)
        {
            ExceptionLogger.LogPacketException(packet.ToString(), string.Format("High latency in {0}: {1}ms", session.User?.Username ?? session.Connection.GetIp(), timeExecution.TotalMilliseconds));
        }
    }

    /*private async Task ExecutePacketAsync(GameClient session, ClientPacket packet, IPacketEvent pak)
    {
        if (this._cancellationTokenSource.IsCancellationRequested)
        {
            return;
        }

        var task = new Task(() => pak.Parse(session, packet));
        task.Start();

        await task.WaitAsync(this._maximumRunTimeInSec, this._cancellationTokenSource.Token).ContinueWith(t =>
        {
            if (t.IsFaulted && t.Exception != null)
            {
                foreach (var e in t.Exception.Flatten().InnerExceptions)
                {
                    var messageError = string.Format("Error handling packet {0} for session {1} @ User Name {2}: {3}", packet.Id, session.ConnectionID, session.GetUser()?.Username ?? string.Empty, e.Message);
                    ExceptionLogger.LogPacketException(packet.Id.ToString(), messageError);
                }
            }
        });
    }*/

    public void UnregisterAll() => this._incomingPackets.Clear();

    private void RegisterCustom()
    {
        this._incomingPackets.Add(ClientPacketHeader.SEND_ALERT, new SendHotelAlertEvent());
        this._incomingPackets.Add(ClientPacketHeader.EDIT_TV, new EditTvYoutubeEvent());
        this._incomingPackets.Add(ClientPacketHeader.MOVE_AVATAR_KEYBOARD, new MoveAvatarKeyboardEvent());
        this._incomingPackets.Add(ClientPacketHeader.RP_BUY_ITEMS, new RpBuyItemsEvent());
        this._incomingPackets.Add(ClientPacketHeader.RP_USE_ITEMS, new RpUseItemsEvent());
        this._incomingPackets.Add(ClientPacketHeader.RP_TROC_ADD_ITEM, new RpTrocAddItemEvent());
        this._incomingPackets.Add(ClientPacketHeader.RP_TROC_REMOVE_ITEM, new RpTrocRemoveItemEvent());
        this._incomingPackets.Add(ClientPacketHeader.RP_TROC_ACCEPTE, new RpTrocAccepteEvent());
        this._incomingPackets.Add(ClientPacketHeader.RP_TROC_CONFIRME, new RpTrocConfirmeEvent());
        this._incomingPackets.Add(ClientPacketHeader.RP_TROC_STOP, new RpTrocStopEvent());
        this._incomingPackets.Add(ClientPacketHeader.BOT_CHOOSE, new RpBotChooseEvent());
    }

    private void RegisterCampaign()
    {
        this._incomingPackets.Add(ClientPacketHeader.OPEN_CAMPAIGN_CALENDAR_DOOR, new OpenCampaignCalendarDoorEvent());
        this._incomingPackets.Add(ClientPacketHeader.OPEN_CAMPAIGN_CALENDAR_DOOR_STAFF, new OpenCampaignCalendarDoorAsStaffEvent());
    }

    private void RegisterHandshake()
    {
        this._incomingPackets.Add(ClientPacketHeader.RELEASE_VERSION, new GetClientVersionEvent());
        this._incomingPackets.Add(ClientPacketHeader.SECURITY_TICKET, new SSOTicketEvent());
        this._incomingPackets.Add(ClientPacketHeader.USER_INFO, new InfoRetrieveEvent());
        this._incomingPackets.Add(ClientPacketHeader.CLIENT_PONG, new PingEvent());

        this._incomingPackets.Add(ClientPacketHeader.SECURITY_MACHINE, new UniqueIDEvent());
    }

    private void RegisterLandingView()
    {
        this._incomingPackets.Add(ClientPacketHeader.GET_CURRENT_TIMING_CODE, new RefreshCampaignEvent());
        this._incomingPackets.Add(ClientPacketHeader.DESKTOP_NEWS, new GetPromoArticlesEvent());
        this._incomingPackets.Add(ClientPacketHeader.GET_COMMUNITY_GOAL_HALL_OF_FAME, new GetCommunityGoalHallOfFameEvent());
    }

    private void RegisterNux() => this._incomingPackets.Add(ClientPacketHeader.USER_NUX_EVENT, new RoomNuxAlertEvent());

    private void RegisterCatalog()
    {
        this._incomingPackets.Add(ClientPacketHeader.GET_CATALOG_INDEX, new GetCatalogIndexEvent());
        this._incomingPackets.Add(ClientPacketHeader.GET_CATALOG_PAGE, new GetCatalogPageEvent());
        this._incomingPackets.Add(ClientPacketHeader.GET_PRODUCT_OFFER, new GetCatalogOfferEvent());
        this._incomingPackets.Add(ClientPacketHeader.CATALOG_PURCHASE, new PurchaseFromCatalogEvent());
        this._incomingPackets.Add(ClientPacketHeader.CATALOG_PURCHASE_GIFT, new PurchaseFromCatalogAsGiftEvent());
        this._incomingPackets.Add(ClientPacketHeader.GET_GIFT_WRAPPING_CONFIG, new GetGiftWrappingConfigurationEvent());
        this._incomingPackets.Add(ClientPacketHeader.APPROVE_NAME, new CheckPetNameEvent());
        this._incomingPackets.Add(ClientPacketHeader.CATALOG_REDEEM_VOUCHER, new RedeemVoucherEvent());
        this._incomingPackets.Add(ClientPacketHeader.CATALOG_REQUESET_PET_BREEDS, new GetSellablePetBreedsEvent());
        this._incomingPackets.Add(ClientPacketHeader.GROUP_MEMBERSHIPS, new GetGroupFurniConfigEvent());
        this._incomingPackets.Add(ClientPacketHeader.MARKETPLACE_CONFIG, new GetMarketplaceConfigurationEvent());
        this._incomingPackets.Add(ClientPacketHeader.GET_CLUB_OFFERS, new GetClubOffersEvent());
        this._incomingPackets.Add(ClientPacketHeader.CATALOG_SELECT_VIP_GIFT, new SelectVipGiftEvent());
    }

    private void RegisterCamera()
    {
        this._incomingPackets.Add(ClientPacketHeader.REQUEST_CAMERA_CONFIGURATION, new RequestCameraConfigurationEvent());
        this._incomingPackets.Add(ClientPacketHeader.RENDER_ROOM_THUMBNAIL, new RenderRoomThumbnailEvent());
        this._incomingPackets.Add(ClientPacketHeader.RENDER_ROOM, new RenderRoomEvent());
        this._incomingPackets.Add(ClientPacketHeader.PURCHASE_PHOTO, new PurchasePhotoEvent());
        this._incomingPackets.Add(ClientPacketHeader.PUBLISH_PHOTO, new PublishPhotoEvent());
        this._incomingPackets.Add(ClientPacketHeader.PHOTO_COMPETITION, new PhotoCompetitionEvent());
    }

    private void RegisterMarketplace()
    {
        this._incomingPackets.Add(ClientPacketHeader.MARKETPLACE_REQUEST_OFFERS, new GetOffersEvent());
        this._incomingPackets.Add(ClientPacketHeader.MARKETPLACE_REQUEST_OWN_ITEMS, new GetOwnOffersEvent());
        this._incomingPackets.Add(ClientPacketHeader.REQUEST_SELL_ITEM, new GetMarketplaceCanMakeOfferEvent());
        this._incomingPackets.Add(ClientPacketHeader.REQUEST_MARKETPLACE_ITEM_STATS, new GetMarketplaceItemStatsEvent());
        this._incomingPackets.Add(ClientPacketHeader.MARKETPLACE_SELL_ITEM, new MakeOfferEvent());
        this._incomingPackets.Add(ClientPacketHeader.MARKETPLACE_TAKE_BACK_ITEM, new CancelOfferEvent());
        this._incomingPackets.Add(ClientPacketHeader.MARKETPLACE_BUY_OFFER, new BuyOfferEvent());
        this._incomingPackets.Add(ClientPacketHeader.MARKETPLACE_REDEEM_CREDITS, new RedeemOfferCreditsEvent());
    }

    private void RegisterNavigator()
    {
        this._incomingPackets.Add(ClientPacketHeader.NAVIGATOR_INIT, new InitializeNewNavigatorEvent());
        this._incomingPackets.Add(ClientPacketHeader.NAVIGATOR_SEARCH, new NavigatorSearchEvent());
        this._incomingPackets.Add(ClientPacketHeader.NAVIGATOR_SETTINGS, new NavigatorSettingsEvent());
        this._incomingPackets.Add(ClientPacketHeader.NAVIGATOR_CATEGORIES, new GetUserFlatCatsEvent());
    }

    private void RegisterRoomConnection()
    {
        this._incomingPackets.Add(ClientPacketHeader.DESKTOP_VIEW, new GoToHotelViewEvent());
        this._incomingPackets.Add(ClientPacketHeader.ROOM_ENTER, new OpenFlatConnectionEvent());
        this._incomingPackets.Add(ClientPacketHeader.GO_TO_FLAT, new GoToFlatEvent());
    }

    private void RegisterRoomSettings()
    {
        this._incomingPackets.Add(ClientPacketHeader.ROOM_SETTINGS, new GetRoomSettingsEvent());
        this._incomingPackets.Add(ClientPacketHeader.ROOM_SETTINGS_SAVE, new SaveRoomSettingsEvent());
        this._incomingPackets.Add(ClientPacketHeader.ROOM_DELETE, new DeleteRoomEvent());
        this._incomingPackets.Add(ClientPacketHeader.ROOM_MUTE, new ToggleMuteToolEvent());


        this._incomingPackets.Add(ClientPacketHeader.ROOM_RIGHTS_LIST, new GetRoomRightsEvent());
        this._incomingPackets.Add(ClientPacketHeader.ROOM_BAN_LIST, new GetRoomBannedUsersEvent());
        this._incomingPackets.Add(ClientPacketHeader.ROOM_BAN_REMOVE, new UnbanUserFromRoomEvent());

    }

    private void FloorPlanEditor()
    {
        this._incomingPackets.Add(ClientPacketHeader.ROOM_MODEL_SAVE, new SaveFloorPlanModelEvent());
        this._incomingPackets.Add(ClientPacketHeader.GET_ROOM_ENTRY_TILE, new InitializeFloorPlanSessionEvent());
        this._incomingPackets.Add(ClientPacketHeader.GET_OCCUPIED_TILES, new GetOccupiedTilesEvent());
    }

    private void RegisterAvatar()
    {
        this._incomingPackets.Add(ClientPacketHeader.GET_WARDROBE, new GetWardrobeEvent());
        this._incomingPackets.Add(ClientPacketHeader.SAVE_WARDROBE_OUTFIT, new SaveWardrobeOutfitEvent());
    }

    private void RegisterRoomAction()
    {
        this._incomingPackets.Add(ClientPacketHeader.ROOM_DOORBELL, new LetUserInEvent());
        this._incomingPackets.Add(ClientPacketHeader.ROOM_BAN_GIVE, new BanUserEvent());
        this._incomingPackets.Add(ClientPacketHeader.ROOM_KICK, new KickUserEvent());
        this._incomingPackets.Add(ClientPacketHeader.ROOM_RIGHTS_GIVE, new AssignRightsEvent());
        this._incomingPackets.Add(ClientPacketHeader.ROOM_RIGHTS_REMOVE, new RemoveRightsEvent());
        this._incomingPackets.Add(ClientPacketHeader.ROOM_RIGHTS_REMOVE_ALL, new RemoveAllRightsEvent());
        this._incomingPackets.Add(ClientPacketHeader.ROOM_MUTE_USER, new MuteUserEvent());
        this._incomingPackets.Add(ClientPacketHeader.UNIT_GIVE_HANDITEM, new GiveHandItemEvent());
        this._incomingPackets.Add(ClientPacketHeader.ROOM_RIGHTS_REMOVE_OWN, new RemoveMyRightsEvent());
    }

    private void RegisterRoomEngine()
    {
        this._incomingPackets.Add(ClientPacketHeader.ROOM_MODEL, new GetRoomEntryDataEvent());
        this._incomingPackets.Add(ClientPacketHeader.FURNITURE_ALIASES, new GetFurnitureAliasesMessageEvent());
        this._incomingPackets.Add(ClientPacketHeader.UNIT_WALK, new MoveAvatarEvent());
        this._incomingPackets.Add(ClientPacketHeader.FURNITURE_FLOOR_UPDATE, new MoveObjectEvent());
        this._incomingPackets.Add(ClientPacketHeader.FURNITURE_PICKUP, new PickupObjectEvent());
        this._incomingPackets.Add(ClientPacketHeader.FURNITURE_PICKUP_ALL, new PickupObjectAllEvent());
        this._incomingPackets.Add(ClientPacketHeader.FURNITURE_WALL_UPDATE, new MoveWallItemEvent());
        this._incomingPackets.Add(ClientPacketHeader.ITEM_PAINT, new ApplyDecorationEvent());
        this._incomingPackets.Add(ClientPacketHeader.FURNITURE_PLACE, new PlaceObjectEvent());
        this._incomingPackets.Add(ClientPacketHeader.FURNITURE_MULTISTATE, new UseFurnitureEvent());
        this._incomingPackets.Add(ClientPacketHeader.FURNITURE_WALL_MULTISTATE, new UseFurnitureEvent());
        this._incomingPackets.Add(ClientPacketHeader.FURNITURE_RANDOMSTATE, new UseFurnitureEvent());
        this._incomingPackets.Add(ClientPacketHeader.POLL_ANSWER, new AnswerPollEvent());
        this._incomingPackets.Add(ClientPacketHeader.ROOM_FAVORITE, new AddFavouriteRoomEvent());
        this._incomingPackets.Add(ClientPacketHeader.ROOM_FAVORITE_REMOVE, new RemoveFavouriteRoomEvent());
        this._incomingPackets.Add(ClientPacketHeader.USER_HOME_ROOM, new NavigatorHomeRoomEvent());
        this._incomingPackets.Add(ClientPacketHeader.CAN_CREATE_ROOM_MESSAGE_EVENT, new CanCreateRoomEvent());
        this._incomingPackets.Add(ClientPacketHeader.ROOM_CREATE, new CreateFlatEvent());
        this._incomingPackets.Add(ClientPacketHeader.ROOM_INFO, new GetGuestRoomEvent());
    }

    private void RegisterRoomChat()
    {
        this._incomingPackets.Add(ClientPacketHeader.UNIT_CHAT, new ChatEvent());
        this._incomingPackets.Add(ClientPacketHeader.UNIT_CHAT_SHOUT, new ChatEvent(true));
        this._incomingPackets.Add(ClientPacketHeader.UNIT_CHAT_WHISPER, new WhisperEvent());
        this._incomingPackets.Add(ClientPacketHeader.UNIT_CHAT_AUDIO, new ChatAudioEvent());
        this._incomingPackets.Add(ClientPacketHeader.UNIT_TYPING, new StartTypingEvent());
        this._incomingPackets.Add(ClientPacketHeader.UNIT_TYPING_STOP, new CancelTypingEvent());
    }

    private void RegisterInventory()
    {
        this._incomingPackets.Add(ClientPacketHeader.TRADE, new InitTradeEvent());
        this._incomingPackets.Add(ClientPacketHeader.TRADE_ITEM, new TradingOfferItemEvent());
        this._incomingPackets.Add(ClientPacketHeader.TRADE_ITEMS, new TradeOfferMultipleItemsEvent());

        this._incomingPackets.Add(ClientPacketHeader.TRADE_ITEM_REMOVE, new TradingRemoveItemEvent());
        this._incomingPackets.Add(ClientPacketHeader.TRADE_ACCEPT, new TradingAcceptEvent());
        this._incomingPackets.Add(ClientPacketHeader.TRADE_CLOSE, new TradingCancelEvent());
        this._incomingPackets.Add(ClientPacketHeader.TRADE_CONFIRM, new TradingConfirmEvent());
        this._incomingPackets.Add(ClientPacketHeader.TRADE_UNACCEPT, new TradingModifyEvent());
        this._incomingPackets.Add(ClientPacketHeader.TRADE_CANCEL, new TradingCancelConfirmEvent());

        this._incomingPackets.Add(ClientPacketHeader.USER_FURNITURE, new RequestFurniInventoryEvent());
        this._incomingPackets.Add(ClientPacketHeader.USER_BADGES, new GetBadgesEvent());
        this._incomingPackets.Add(ClientPacketHeader.ACHIEVEMENT_LIST, new GetAchievementsEvent());
        this._incomingPackets.Add(ClientPacketHeader.USER_BADGES_CURRENT_UPDATE, new SetActivatedBadgesEvent());
        this._incomingPackets.Add(ClientPacketHeader.USER_BOTS, new GetBotInventoryEvent());
        this._incomingPackets.Add(ClientPacketHeader.USER_PETS, new GetPetInventoryEvent());
        this._incomingPackets.Add(ClientPacketHeader.USER_EFFECT_ACTIVATE, new AvatarEffectActivatedEvent());
        this._incomingPackets.Add(ClientPacketHeader.USER_EFFECT_ENABLE, new AvatarEffectSelectedEvent());

        this._incomingPackets.Add(ClientPacketHeader.DELETE_BADGE_INVENTORY, new DeleteBadgeInventoryEvent());
        this._incomingPackets.Add(ClientPacketHeader.DELETE_FURNITURE_TYPE_INVENTORY, new DeleteFurniTypeInventoryEvent());
    }

    private void RegisterPurse() => this._incomingPackets.Add(ClientPacketHeader.USER_CURRENCY, new GetCreditsInfoEvent());

    private void RegisterMessenger()
    {
        this._incomingPackets.Add(ClientPacketHeader.MESSENGER_INIT, new MessengerInitEvent());
        this._incomingPackets.Add(ClientPacketHeader.GET_FRIEND_REQUESTS, new GetBuddyRequestsEvent());
        this._incomingPackets.Add(ClientPacketHeader.FOLLOW_FRIEND, new FollowFriendEvent());
        this._incomingPackets.Add(ClientPacketHeader.FIND_NEW_FRIENDS, new FindNewFriendsEvent());

        this._incomingPackets.Add(ClientPacketHeader.REMOVE_FRIEND, new RemoveBuddyEvent());
        this._incomingPackets.Add(ClientPacketHeader.REQUEST_FRIEND, new RequestBuddyEvent());
        this._incomingPackets.Add(ClientPacketHeader.MESSENGER_CHAT, new SendMsgEvent());
        this._incomingPackets.Add(ClientPacketHeader.SEND_ROOM_INVITE, new SendRoomInviteEvent());
        this._incomingPackets.Add(ClientPacketHeader.HABBO_SEARCH, new UserSearchEvent());
        this._incomingPackets.Add(ClientPacketHeader.ACCEPT_FRIEND, new AcceptBuddyEvent());
        this._incomingPackets.Add(ClientPacketHeader.DECLINE_FRIEND, new DeclineBuddyEvent());
    }

    private void RegisterGroups()
    {
        this._incomingPackets.Add(ClientPacketHeader.GROUP_REQUEST, new JoinGroupEvent());
        this._incomingPackets.Add(ClientPacketHeader.GROUP_UNFAVORITE, new RemoveGroupFavouriteEvent());
        this._incomingPackets.Add(ClientPacketHeader.GROUP_FAVORITE, new SetGroupFavouriteEvent());
        this._incomingPackets.Add(ClientPacketHeader.GROUP_INFO, new GetGroupInfoEvent());
        this._incomingPackets.Add(ClientPacketHeader.GROUP_MEMBERS, new GetGroupMembersEvent());
        this._incomingPackets.Add(ClientPacketHeader.GROUP_CREATE_OPTIONS, new GetGroupCreationWindowEvent());
        this._incomingPackets.Add(ClientPacketHeader.GROUP_PARTS, new GetBadgeEditorPartsEvent());
        this._incomingPackets.Add(ClientPacketHeader.GROUP_BUY, new PurchaseGroupEvent());
        this._incomingPackets.Add(ClientPacketHeader.GROUP_SAVE_INFORMATION, new UpdateGroupIdentityEvent());
        this._incomingPackets.Add(ClientPacketHeader.GROUP_SAVE_BADGE, new UpdateGroupBadgeEvent());
        this._incomingPackets.Add(ClientPacketHeader.GROUP_SAVE_COLORS, new UpdateGroupColoursEvent());
        this._incomingPackets.Add(ClientPacketHeader.GROUP_SAVE_PREFERENCES, new UpdateGroupSettingsEvent());
        this._incomingPackets.Add(ClientPacketHeader.GROUP_SETTINGS, new ManageGroupEvent());
        this._incomingPackets.Add(ClientPacketHeader.GROUP_ADMIN_ADD, new GiveAdminRightsEvent());
        this._incomingPackets.Add(ClientPacketHeader.GROUP_ADMIN_REMOVE, new TakeAdminRightsEvent());
        this._incomingPackets.Add(ClientPacketHeader.GROUP_MEMBER_REMOVE_CONFIRM, new RemoveGroupMemberEvent());
        this._incomingPackets.Add(ClientPacketHeader.GROUP_MEMBER_REMOVE, new RemoveGroupMemberEvent());
        this._incomingPackets.Add(ClientPacketHeader.GROUP_REQUEST_ACCEPT, new AcceptGroupMembershipEvent());
        this._incomingPackets.Add(ClientPacketHeader.GROUP_REQUEST_DECLINE, new DeclineGroupMembershipEvent());
        this._incomingPackets.Add(ClientPacketHeader.GROUP_DELETE, new DeleteGroupEvent());
    }

    private void RegisterPets()
    {
        this._incomingPackets.Add(ClientPacketHeader.PET_RESPECT, new RespectPetEvent());
        this._incomingPackets.Add(ClientPacketHeader.PET_INFO, new GetPetInformationEvent());
        this._incomingPackets.Add(ClientPacketHeader.PET_PICKUP, new PickUpPetEvent());
        this._incomingPackets.Add(ClientPacketHeader.PET_PLACE, new PlacePetEvent());
        this._incomingPackets.Add(ClientPacketHeader.PET_RIDE, new RideHorseEvent());
        this._incomingPackets.Add(ClientPacketHeader.USE_PET_PRODUCT, new ApplyHorseEffectEvent());
        this._incomingPackets.Add(ClientPacketHeader.REMOVE_PET_SADDLE, new RemoveSaddleFromHorseEvent());
        this._incomingPackets.Add(ClientPacketHeader.TOGGLE_PET_RIDING, new ModifyWhoCanRideHorseEvent());
        this._incomingPackets.Add(ClientPacketHeader.GET_PET_TRAINING_PANEL_MESSAGE_EVENT, new GetPetTrainingPanelEvent());
        this._incomingPackets.Add(ClientPacketHeader.PET_MOVE, new MoveMonsterPlanteEvent());
    }

    private void RegisterQuests()
    {
        this._incomingPackets.Add(ClientPacketHeader.GET_QUESTS, new GetQuestListEvent());
        this._incomingPackets.Add(ClientPacketHeader.ACCEPT_QUEST, new StartQuestEvent());
        this._incomingPackets.Add(ClientPacketHeader.REJECT_QUEST, new CancelQuestEvent());
        this._incomingPackets.Add(ClientPacketHeader.OPEN_QUEST_TRACKER, new GetCurrentQuestEvent());
    }

    private void RegisterFurni()
    {
        this._incomingPackets.Add(ClientPacketHeader.ITEM_STACK_HELPER, new UpdateMagicTileEvent());
        this._incomingPackets.Add(ClientPacketHeader.WIRED_TRIGGER_SAVE, new UpdateTriggerEvent());
        this._incomingPackets.Add(ClientPacketHeader.WIRED_ACTION_SAVE, new UpdateActionEvent());
        this._incomingPackets.Add(ClientPacketHeader.WIRED_CONDITION_SAVE, new UpdateConditionEvent());
        this._incomingPackets.Add(ClientPacketHeader.SET_OBJECT_DATA, new SaveBrandingItemEvent());
        this._incomingPackets.Add(ClientPacketHeader.ROOM_TONER_APPLY, new SetTonerEvent());
        this._incomingPackets.Add(ClientPacketHeader.ITEM_DICE_CLOSE, new DiceOffEvent());
        this._incomingPackets.Add(ClientPacketHeader.ITEM_DICE_CLICK, new UseFurnitureEvent());
        this._incomingPackets.Add(ClientPacketHeader.MANNEQUIN_SAVE_NAME, new SetMannequinNameEvent());
        this._incomingPackets.Add(ClientPacketHeader.MANNEQUIN_SAVE_LOOK, new SetMannequinFigureEvent());
        this._incomingPackets.Add(ClientPacketHeader.ITEM_EXCHANGE_REDEEM, new CreditFurniRedeemEvent());
        this._incomingPackets.Add(ClientPacketHeader.GET_ITEM_DATA, new GetStickyNoteEvent());
        this._incomingPackets.Add(ClientPacketHeader.FURNITURE_POSTIT_PLACE, new AddStickyNoteEvent());
        this._incomingPackets.Add(ClientPacketHeader.SET_ITEM_DATA, new UpdateStickyNoteEvent());
        this._incomingPackets.Add(ClientPacketHeader.REMOVE_WALL_ITEM, new DeleteStickyNoteEvent());
        this._incomingPackets.Add(ClientPacketHeader.ITEM_DIMMER_SETTINGS, new GetMoodlightConfigEvent());
        this._incomingPackets.Add(ClientPacketHeader.ITEM_DIMMER_SAVE, new MoodlightUpdateEvent());
        this._incomingPackets.Add(ClientPacketHeader.ITEM_DIMMER_TOGGLE, new ToggleMoodlightEvent());
        this._incomingPackets.Add(ClientPacketHeader.ONE_WAY_DOOR_CLICK, new UseFurnitureEvent());
        this._incomingPackets.Add(ClientPacketHeader.ITEM_COLOR_WHEEL_CLICK, new UseFurnitureEvent());
        this._incomingPackets.Add(ClientPacketHeader.FOOTBALL_GATE_SAVE_LOOK_EVENT, new ChangeFootGate());
        this._incomingPackets.Add(ClientPacketHeader.PRESENT_OPEN_PRESENT, new OpenGiftEvent());
        this._incomingPackets.Add(ClientPacketHeader.FURNITURE_GROUP_INFO, new GetGroupFurniSettingsEvent());
        this._incomingPackets.Add(ClientPacketHeader.FRIEND_FURNI_CONFIRM_LOCK, new ConfirmLoveLockEvent());
    }

    private void RegisterUsers()
    {
        this._incomingPackets.Add(ClientPacketHeader.USER_SUBSCRIPTION, new ScrGetUserInfoMessageEvent());
        this._incomingPackets.Add(ClientPacketHeader.USER_RESPECT, new RespectUserEvent());
        this._incomingPackets.Add(ClientPacketHeader.USER_FIGURE, new UpdateFigureDataEvent());
        this._incomingPackets.Add(ClientPacketHeader.USER_PROFILE, new OpenPlayerProfileEvent());
        this._incomingPackets.Add(ClientPacketHeader.USER_BADGES_CURRENT, new GetSelectedBadgesEvent());
        this._incomingPackets.Add(ClientPacketHeader.MESSENGER_RELATIONSHIPS, new GetRelationshipsEvent());
        this._incomingPackets.Add(ClientPacketHeader.SET_RELATIONSHIP_STATUS, new SetRelationshipEvent());
        this._incomingPackets.Add(ClientPacketHeader.CHECK_USERNAME, new CheckValidNameEvent());
        this._incomingPackets.Add(ClientPacketHeader.CHANGE_USERNAME, new ChangeNameEvent());
        this._incomingPackets.Add(ClientPacketHeader.GROUP_BADGES, new GetUserGroupBadgesEvent());
        this._incomingPackets.Add(ClientPacketHeader.USER_BANNER, new UserBannerEvent());
        this._incomingPackets.Add(ClientPacketHeader.USER_BANNER_SELECT, new UserBannerSelectEvent());
    }

    private void RegisterSettings()
    {
        this._incomingPackets.Add(ClientPacketHeader.USER_SETTINGS_OLD_CHAT, new UserSettingsOldChatEvent());
        this._incomingPackets.Add(ClientPacketHeader.USER_SETTINGS_INVITES, new UserSettingsRoomInvitesEvent());
        this._incomingPackets.Add(ClientPacketHeader.USER_SETTINGS_CAMERA, new UserSettingsCameraFollowEvent());
        this._incomingPackets.Add(ClientPacketHeader.USER_SETTINGS_VOLUME, new UserSettingsSoundEvent());
    }

    private void RegisterMisc()
    {
        this._incomingPackets.Add(ClientPacketHeader.CLIENT_LATENCY, new LatencyTestEvent());
        this._incomingPackets.Add(ClientPacketHeader.CLIENT_TOOLBAR_TOGGLE, new SetFriendBarStateEvent());
    }

    private void RegisterRoomAvatar()
    {
        this._incomingPackets.Add(ClientPacketHeader.UNIT_ACTION, new ActionEvent());
        this._incomingPackets.Add(ClientPacketHeader.UNIT_SIGN, new ApplySignEvent());
        this._incomingPackets.Add(ClientPacketHeader.UNIT_DANCE, new DanceEvent());
        this._incomingPackets.Add(ClientPacketHeader.UNIT_POSTURE, new SitEvent());
        this._incomingPackets.Add(ClientPacketHeader.USER_MOTTO, new ChangeMottoEvent());
        this._incomingPackets.Add(ClientPacketHeader.UNIT_LOOK, new LookToEvent());
        this._incomingPackets.Add(ClientPacketHeader.UNIT_DROP_HAND_ITEM, new DropHandItemEvent());
        this._incomingPackets.Add(ClientPacketHeader.ROOM_LIKE, new GiveRoomScoreEvent());
        this._incomingPackets.Add(ClientPacketHeader.USER_IGNORE, new IgnoreUserEvent());
        this._incomingPackets.Add(ClientPacketHeader.USER_UNIGNORE, new UnIgnoreUserEvent());
        this._incomingPackets.Add(ClientPacketHeader.UNIT_CHAT_WHISPER_GROUP, new WhiperGroupEvent());
    }

    private void RegisterBots()
    {
        this._incomingPackets.Add(ClientPacketHeader.BOT_PLACE, new PlaceBotEvent());
        this._incomingPackets.Add(ClientPacketHeader.BOT_PICKUP, new PickUpBotEvent());
        this._incomingPackets.Add(ClientPacketHeader.BOT_CONFIGURATION, new OpenBotActionEvent());
        this._incomingPackets.Add(ClientPacketHeader.BOT_SKILL_SAVE, new SaveBotActionEvent());
    }

    private void RegisterModeration()
    {
        this._incomingPackets.Add(ClientPacketHeader.GET_PENDING_CALLS_FOR_HELP, new OpenHelpToolEvent());
        this._incomingPackets.Add(ClientPacketHeader.MODTOOL_REQUEST_ROOM_INFO, new GetModeratorRoomInfoEvent());
        this._incomingPackets.Add(ClientPacketHeader.MOD_TOOL_USER_INFO, new GetModeratorUserInfoEvent());
        this._incomingPackets.Add(ClientPacketHeader.MODTOOL_REQUEST_USER_ROOMS, new GetModeratorUserRoomVisitsEvent());
        this._incomingPackets.Add(ClientPacketHeader.MODTOOL_CHANGE_ROOM_SETTINGS, new ModerateRoomEvent());
        this._incomingPackets.Add(ClientPacketHeader.MODTOOL_ROOM_ALERT, new ModeratorActionEvent());
        this._incomingPackets.Add(ClientPacketHeader.CALL_FOR_HELP, new SubmitNewTicketEvent());
        this._incomingPackets.Add(ClientPacketHeader.MODTOOL_REQUEST_ROOM_CHATLOG, new GetModeratorRoomChatlogEvent());
        this._incomingPackets.Add(ClientPacketHeader.MODTOOL_REQUEST_USER_CHATLOG, new GetModeratorUserChatlogEvent());

        this._incomingPackets.Add(ClientPacketHeader.PICK_ISSUES, new PickTicketEvent());
        this._incomingPackets.Add(ClientPacketHeader.RELEASE_ISSUES, new ReleaseTicketEvent());
        this._incomingPackets.Add(ClientPacketHeader.CLOSE_ISSUES, new CloseTicketEvent());

        this._incomingPackets.Add(ClientPacketHeader.MODTOOL_SANCTION_MUTE, new ModerationMuteEvent());
        this._incomingPackets.Add(ClientPacketHeader.MODTOOL_SANCTION_KICK, new ModerationMuteEvent());
        this._incomingPackets.Add(ClientPacketHeader.MODTOOL_SANCTION_BAN, new ModerationBanEvent());
        this._incomingPackets.Add(ClientPacketHeader.MODTOOL_ALERTEVENT, new ModerationMsgEvent());
        this._incomingPackets.Add(ClientPacketHeader.MODTOOL_SANCTION_ALERT, new ModerationMsgEvent());
    }

    private void RegisterGuide()
    {
        this._incomingPackets.Add(ClientPacketHeader.GUIDE_SESSION_ON_DUTY_UPDATE, new GetHelperToolConfigurationEvent());
        this._incomingPackets.Add(ClientPacketHeader.GUIDE_SESSION_GUIDE_DECIDES, new OnGuideSessionDetachedEvent());
        this._incomingPackets.Add(ClientPacketHeader.GUIDE_SESSION_CREATE, new OnGuideEvent());
        this._incomingPackets.Add(ClientPacketHeader.GUIDE_SESSION_FEEDBACK, new RecomendHelpersEvent());
        this._incomingPackets.Add(ClientPacketHeader.GUIDE_SESSION_MESSAGE, new GuideToolMessageNewEvent());
        this._incomingPackets.Add(ClientPacketHeader.GUIDE_SESSION_INVITE_REQUESTER, new GuideInviteToRoomEvent());
        this._incomingPackets.Add(ClientPacketHeader.GUIDE_SESSION_GET_REQUESTER_ROOM, new VisitRoomGuidesEvent());
        this._incomingPackets.Add(ClientPacketHeader.GUIDE_SESSION_RESOLVED, new GuideEndSessionEvent());
        this._incomingPackets.Add(ClientPacketHeader.GUIDE_SESSION_REQUESTER_CANCELS, new CancellInviteGuideEvent());
    }
}
