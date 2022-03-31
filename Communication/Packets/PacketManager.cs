using Butterfly.Communication.Packets.Incoming;
using Butterfly.Communication.Packets.Incoming.Marketplace;
using Butterfly.Communication.Packets.Incoming.Structure;
using Butterfly.Communication.Packets.Incoming.Camera;
using Butterfly.Communication.Packets.Incoming.Campaign;
using Butterfly.Communication.Packets.Incoming.WebSocket;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.WebClients;
using System;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets
{
    public sealed class PacketManager
    {
        private readonly Dictionary<int, IPacketEvent> _incomingPackets;
        private readonly Dictionary<int, IPacketWebEvent> _incomingWebPackets;

        public PacketManager()
        {
            this._incomingPackets = new Dictionary<int, IPacketEvent>();
            this._incomingWebPackets = new Dictionary<int, IPacketWebEvent>();
        }

        public void Init(IQueryAdapter dbClient)
        {
            this.UnregisterAll();

            this.RegisterHandshake();
            this.RegisterLandingView();
            this.RegisterCatalog();
            this.RegisterNavigator();
            this.RegisterMarketplace();
            this.RegisterNewNavigator();
            this.RegisterRoomAction();
            this.RegisterQuests();
            this.RegisterRoomConnection();
            this.RegisterRoomChat();
            this.RegisterRoomEngine();
            this.RegisterFurni();
            this.RegisterUsers();
            this.RegisterSound();
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
            this.RegisterForum();
            this.RegisterCamera();
            this.RegisterCampaign();

            this.RegisterWebPacket();

            Console.WriteLine("Logged " + this._incomingPackets.Count + " packet handler(s)!");
        }

        public void TryExecutePacket(Client session, ClientPacket packet)
        {
            if (!this._incomingPackets.TryGetValue(packet.Id, out IPacketEvent pak))
            {
                if (ButterflyEnvironment.StaticEvents)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(packet.ToString());
                    Console.ResetColor();
                }
                return;
            }

            if (ButterflyEnvironment.StaticEvents)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(packet.ToString());
                Console.ResetColor();
            }
            
            if(session.PacketTimeout(packet.Id, pak.Delay))
            {
                if (ButterflyEnvironment.StaticEvents)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("[" + packet.Id + "] Spam detected");
                    Console.ResetColor();
                }
                return;
            }

            pak.Parse(session, packet);
        }

        public void TryExecuteWebPacket(WebClient session, ClientPacket packet)
        {
            if (!this._incomingWebPackets.TryGetValue(packet.Id, out IPacketWebEvent pak))
            {
                if (ButterflyEnvironment.StaticEvents)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(packet.ToString());
                    Console.ResetColor();
                }
                return;
            }

            if (ButterflyEnvironment.StaticEvents)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(packet.ToString());
                Console.ResetColor();
            }

            if (session.PacketTimeout(packet.Id, pak.Delay))
            {
                if (ButterflyEnvironment.StaticEvents)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("[" + packet.Id + "] Spam detected");
                    Console.ResetColor();
                }
                return;
            }

            pak.Parse(session, packet);
        }

        private void RegisterWebPacket()
        {
            this._incomingWebPackets.Add(1, new SSOTicketWebEvent());
            this._incomingWebPackets.Add(2, new SendHotelAlertEvent());
            this._incomingWebPackets.Add(3, new EditTvYoutubeEvent());
            this._incomingWebPackets.Add(4, new PingWebEvent());

            this._incomingWebPackets.Add(6, new MoveUserEvent());
            this._incomingWebPackets.Add(7, new FollowUserIdEvent());
            this._incomingWebPackets.Add(8, new RpBuyItemsEvent());
            this._incomingWebPackets.Add(9, new RpUseItemsEvent());
            this._incomingWebPackets.Add(10, new JoinRoomIdEvent());
            this._incomingWebPackets.Add(11, new RpTrocAddItemEvent());
            this._incomingWebPackets.Add(12, new RpTrocRemoveItemEvent());
            this._incomingWebPackets.Add(13, new RpTrocAccepteEvent());
            this._incomingWebPackets.Add(14, new RpTrocConfirmeEvent());
            this._incomingWebPackets.Add(15, new RpTrocStopEvent());
            this._incomingWebPackets.Add(16, new RpBotChooseEvent());
            this._incomingWebPackets.Add(17, new DisconnectWebEvent());
        }

        public void UnregisterAll()
        {
            this._incomingPackets.Clear();
            this._incomingWebPackets.Clear();
        }

        private void RegisterCampaign()
        {
            this._incomingPackets.Add(ClientPacketHeader.OPEN_CAMPAIGN_CALENDAR_DOOR, new OpenCampaignCalendarDoorEvent());
            this._incomingPackets.Add(ClientPacketHeader.OPEN_CAMPAIGN_CALENDAR_DOOR_STAFF, new OpenCampaignCalendarDoorAsStaffEvent());
        }
        private void RegisterForum()
        {
            this._incomingPackets.Add(ClientPacketHeader.GET_FORUM_THREADS, new GuildForumListEvent());
        }

        private void RegisterHandshake()
        {
            this._incomingPackets.Add(ClientPacketHeader.RELEASE_VERSION, new GetClientVersionEvent());
            this._incomingPackets.Add(ClientPacketHeader.SECURITY_TICKET, new SSOTicketEvent());
            this._incomingPackets.Add(ClientPacketHeader.USER_INFO, new InfoRetrieveEvent());
            this._incomingPackets.Add(ClientPacketHeader.CLIENT_PONG, new PingEvent());

            this._incomingPackets.Add(ClientPacketHeader.GenerateSecretKeyMessageEvent, new GenerateSecretKeyEvent());
            this._incomingPackets.Add(ClientPacketHeader.SECURITY_MACHINE, new UniqueIDEvent());
        }

        private void RegisterLandingView()
        {
            this._incomingPackets.Add(ClientPacketHeader.GET_CURRENT_TIMING_CODE, new RefreshCampaignEvent());
            this._incomingPackets.Add(ClientPacketHeader.DESKTOP_NEWS, new GetPromoArticlesEvent());
        }

        private void RegisterNux()
        {
            this._incomingPackets.Add(ClientPacketHeader.UserNuxEvent, new RoomNuxAlertEvent());
        }

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
            this._incomingPackets.Add(ClientPacketHeader.ROOM_FAVORITE, new AddFavouriteRoomEvent());
            this._incomingPackets.Add(ClientPacketHeader.NAVIGATOR_CATEGORIES, new GetUserFlatCatsEvent());
            this._incomingPackets.Add(ClientPacketHeader.ROOM_FAVORITE_REMOVE, new RemoveFavouriteRoomEvent());
            this._incomingPackets.Add(ClientPacketHeader.DESKTOP_VIEW, new GoToHotelViewEvent());
            this._incomingPackets.Add(ClientPacketHeader.USER_HOME_ROOM, new UpdateNavigatorSettingsEvent());
            this._incomingPackets.Add(ClientPacketHeader.CanCreateRoomMessageEvent, new CanCreateRoomEvent());
            this._incomingPackets.Add(ClientPacketHeader.ROOM_CREATE, new CreateFlatEvent());
            this._incomingPackets.Add(ClientPacketHeader.ROOM_INFO, new GetGuestRoomEvent());
        }

        private void RegisterNewNavigator()
        {
            this._incomingPackets.Add(ClientPacketHeader.NAVIGATOR_INIT, new InitializeNewNavigatorEvent());
            this._incomingPackets.Add(ClientPacketHeader.NAVIGATOR_SEARCH, new NavigatorSearchEvent());
            this._incomingPackets.Add(ClientPacketHeader.FindRandomFriendingRoomMessageEvent, new FindRandomFriendingRoomEvent());
        }
        private void RegisterRoomConnection()
        {
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
            this._incomingPackets.Add(ClientPacketHeader.FURNITURE_WALL_UPDATE, new MoveWallItemEvent());
            this._incomingPackets.Add(ClientPacketHeader.ITEM_PAINT, new ApplyDecorationEvent());
            this._incomingPackets.Add(ClientPacketHeader.FURNITURE_PLACE, new PlaceObjectEvent());
            this._incomingPackets.Add(ClientPacketHeader.FURNITURE_MULTISTATE, new UseFurnitureEvent());
            this._incomingPackets.Add(ClientPacketHeader.FURNITURE_WALL_MULTISTATE, new UseFurnitureEvent());
            this._incomingPackets.Add(ClientPacketHeader.POLL_ANSWER, new AnswerPollEvent());
        }

        private void RegisterRoomChat()
        {
            this._incomingPackets.Add(ClientPacketHeader.UNIT_CHAT, new ChatEvent());
            this._incomingPackets.Add(ClientPacketHeader.UNIT_CHAT_SHOUT, new ShoutEvent());
            this._incomingPackets.Add(ClientPacketHeader.UNIT_CHAT_WHISPER, new WhisperEvent());
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
        }

        private void RegisterPurse()
        {
            this._incomingPackets.Add(ClientPacketHeader.USER_CURRENCY, new GetCreditsInfoEvent());
        }

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
            this._incomingPackets.Add(ClientPacketHeader.GetPetTrainingPanelMessageEvent, new GetPetTrainingPanelEvent());
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
            this._incomingPackets.Add(ClientPacketHeader.FootballGateSaveLookEvent, new ChangeFootGate());
            this._incomingPackets.Add(ClientPacketHeader.PRESENT_OPEN_PRESENT, new OpenGiftEvent());
            this._incomingPackets.Add(ClientPacketHeader.FURNITURE_GROUP_INFO, new GetGroupFurniSettingsEvent());

            this._incomingPackets.Add(ClientPacketHeader.FRIEND_FURNI_CONFIRM_LOCK, new ConfirmLoveLockEvent());
        }

        private void RegisterUsers()
        {
            this._incomingPackets.Add(ClientPacketHeader.USER_SUBSCRIPTION, new ScrGetUserInfoMessageEvent());
            this._incomingPackets.Add(ClientPacketHeader.USER_SETTINGS_OLD_CHAT, new SetChatPreferenceEvent());

            this._incomingPackets.Add(ClientPacketHeader.USER_RESPECT, new RespectUserEvent());
            this._incomingPackets.Add(ClientPacketHeader.USER_FIGURE, new UpdateFigureDataEvent());
            this._incomingPackets.Add(ClientPacketHeader.USER_PROFILE, new OpenPlayerProfileEvent());
            this._incomingPackets.Add(ClientPacketHeader.USER_BADGES_CURRENT, new GetSelectedBadgesEvent());
            this._incomingPackets.Add(ClientPacketHeader.MESSENGER_RELATIONSHIPS, new GetRelationshipsEvent());
            this._incomingPackets.Add(ClientPacketHeader.SET_RELATIONSHIP_STATUS, new SetRelationshipEvent());
            this._incomingPackets.Add(ClientPacketHeader.CHECK_USERNAME, new CheckValidNameEvent());
            this._incomingPackets.Add(ClientPacketHeader.CHANGE_USERNAME, new ChangeNameEvent());
            this._incomingPackets.Add(ClientPacketHeader.GROUP_BADGES, new GetUserGroupBadgesEvent());
        }

        private void RegisterSound()
        {
            this._incomingPackets.Add(ClientPacketHeader.USER_SETTINGS_VOLUME, new SetSoundSettingsEvent());
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
}
