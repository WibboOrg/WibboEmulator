using Butterfly.Communication.Packets.Incoming;
using Butterfly.Communication.Packets.Outgoing.Rooms.Wireds;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Map.Movement;
using Butterfly.Game.Rooms.Wired.WiredHandlers;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Conditions;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Effects;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Triggers;
using System.Collections.Generic;
using Butterfly.Database.Daos;
using System.Data;
using System;

namespace Butterfly.Game.Rooms.Wired
{
    public class WiredRegister
    {
        private static IWired GetWiredHandler(Item item, Room room)
        {
            IWired handler = null;
            switch (item.GetBaseItem().InteractionType)
            {
                #region Trigger
                case InteractionType.TRIGGER_ONCE:
                    handler = new Timer(item, room.GetWiredHandler(), 2, room.GetGameManager());
                    break;
                case InteractionType.TRIGGER_AVATAR_ENTERS_ROOM:
                    handler = new EntersRoom(item, room.GetWiredHandler(), room.GetRoomUserManager(), false, string.Empty);
                    break;
                case InteractionType.TRIGGER_COLLISION:
                    handler = new Collision(item, room.GetWiredHandler(), room.GetRoomUserManager());
                    break;
                case InteractionType.TRIGGER_GAME_ENDS:
                    handler = new GameEnds(item, room.GetWiredHandler(), room.GetGameManager());
                    break;
                case InteractionType.TRIGGER_GAME_STARTS:
                    handler = new GameStarts(item, room.GetWiredHandler(), room.GetGameManager());
                    break;
                case InteractionType.TRIGGER_PERIODICALLY:
                    handler = new Repeater(room.GetWiredHandler(), item, 0);
                    break;
                case InteractionType.TRIGGER_PERIODICALLY_LONG:
                    handler = new Repeaterlong(room.GetWiredHandler(), item, 0);
                    break;
                case InteractionType.TRIGGER_AVATAR_SAYS_SOMETHING:
                    handler = new UserSays(item, room.GetWiredHandler(), false, string.Empty, room);
                    break;
                case InteractionType.TRIGGER_COMMAND:
                    handler = new UserCommand(item, room.GetWiredHandler(), room);
                    break;
                case InteractionType.WIRED_TRIGGER_SELF:
                    handler = new UserTriggerSelf(item, room.GetWiredHandler(), room);
                    break;
                case InteractionType.WF_TRG_BOT_REACHED_AVTR:
                    handler = new BotReadchedAvatar(item, room.GetWiredHandler(), "");
                    break;
                case InteractionType.TRIGGER_COLLISION_USER:
                    handler = new UserCollision(item, room.GetWiredHandler(), room);
                    break;
                case InteractionType.TRIGGER_SCORE_ACHIEVED:
                    handler = new ScoreAchieved(item, room.GetWiredHandler(), 0, room.GetGameManager());
                    break;
                case InteractionType.TRIGGER_STATE_CHANGED:
                    handler = new SateChanged(room.GetWiredHandler(), item, new List<Item>(), new List<int>(), 0);
                    break;
                case InteractionType.TRIGGER_WALK_ON_FURNI:
                    handler = new WalksOnFurni(item, room.GetWiredHandler(), new List<Item>(), new List<int>(), 0);
                    break;
                case InteractionType.TRIGGER_WALK_OFF_FURNI:
                    handler = new WalksOffFurni(item, room.GetWiredHandler(), new List<Item>(), new List<int>(), 0);
                    break;
                #endregion
                #region Action
                case InteractionType.ACTIONGIVESCORE:
                    handler = new GiveScore(0, 0, room.GetGameManager(), item.Id);
                    break;
                case InteractionType.WF_ACT_GIVE_SCORE_TM:
                    handler = new GiveScoreTeam(0, 0, 0, room.GetGameManager(), item.Id);
                    break;
                case InteractionType.ACTIONPOSRESET:
                    handler = new PositionReset(new List<Item>(), 0, room.GetRoomItemHandler(), room.GetWiredHandler(), item.Id, 0, 0, 0);
                    break;
                case InteractionType.ACTIONMOVEROTATE:
                    handler = new MoveRotate(0, 0, new List<Item>(), 0, room, room.GetWiredHandler(), item.Id);
                    break;
                case InteractionType.ACTIONRESETTIMER:
                    handler = new TimerReset(room, room.GetWiredHandler(), 1, item.Id);
                    break;
                case InteractionType.HIGHSCORE:
                    handler = new HighScore(item);
                    break;
                case InteractionType.HIGHSCOREPOINTS:
                    handler = new HighScorePoints(item);
                    break;
                case InteractionType.ACTIONSHOWMESSAGE:
                    handler = new ShowMessage(string.Empty, room.GetWiredHandler(), item.Id, 0);
                    break;
                case InteractionType.ACTIONGIVEREWARD:
                    //handler = new GiveReward(string.Empty, room.GetWiredHandler(), item.Id);
                    break;
                case InteractionType.SUPERWIRED:
                    handler = new SuperWired(string.Empty, 0, false, false, room.GetWiredHandler(), item.Id);
                    break;
                case InteractionType.ACTIONTELEPORTTO:
                    handler = new TeleportToItem(room.GetGameMap(), room.GetWiredHandler(), new List<Item>(), 0, item.Id);
                    break;
                case InteractionType.WF_ACT_ENDGAME_TEAM:
                    handler = new TeamGameOver(1, item.Id, room);
                    break;
                case InteractionType.ACTIONTOGGLESTATE:
                    handler = new ToggleItemState(room.GetGameMap(), room.GetWiredHandler(), new List<Item>(), 0, item);
                    break;
                case InteractionType.WF_ACT_CALL_STACKS:
                    handler = new ExecutePile(new List<Item>(), 0, room.GetWiredHandler(), item);
                    break;
                case InteractionType.ACTIONKICKUSER:
                    handler = new KickUser(string.Empty, room.GetWiredHandler(), item.Id, room);
                    break;
                case InteractionType.ACTIONFLEE:
                    handler = new Escape(new List<Item>(), room, room.GetWiredHandler(), item.Id);
                    break;
                case InteractionType.ACTIONCHASE:
                    handler = new Chase(new List<Item>(), room, room.GetWiredHandler(), item.Id);
                    break;
                case InteractionType.COLLISIONTEAM:
                    handler = new CollisionTeam(1, room, room.GetWiredHandler(), item.Id);
                    break;
                case InteractionType.COLLISIONCASE:
                    handler = new CollisionCase(new List<Item>(), room, room.GetWiredHandler(), item.Id);
                    break;
                case InteractionType.ACTIONMOVETODIR:
                    handler = new MoveToDir(new List<Item>(), room, room.GetWiredHandler(), item.Id, 8, 0);
                    break;
                case InteractionType.WF_ACT_BOT_CLOTHES:
                    handler = new BotClothes("", room.GetWiredHandler(), item.Id);
                    break;
                case InteractionType.WF_ACT_BOT_TELEPORT:
                    handler = new BotTeleport("", new List<Item>(), room.GetGameMap(), room.GetWiredHandler(), item.Id);
                    break;
                case InteractionType.WF_ACT_BOT_FOLLOW_AVATAR:
                    handler = new BotFollowAvatar("", false, room.GetWiredHandler(), item.Id);
                    break;
                case InteractionType.WF_ACT_BOT_GIVE_HANDITEM:
                    handler = new BotGiveHanditem("", room.GetWiredHandler(), item.Id);
                    break;
                case InteractionType.WF_ACT_BOT_MOVE:
                    handler = new BotMove("", new List<Item>(), room.GetWiredHandler(), item.Id);
                    break;
                case InteractionType.WF_ACT_USER_MOVE:
                    handler = new UserMove(new List<Item>(), 0, room.GetWiredHandler(), item.Id);
                    break;
                case InteractionType.WF_ACT_BOT_TALK_TO_AVATAR:
                    handler = new BotTalkToAvatar("", false, room.GetWiredHandler(), item.Id);
                    break;
                case InteractionType.WF_ACT_BOT_TALK:
                    handler = new BotTalk("", false, room.GetWiredHandler(), item.Id);
                    break;
                case InteractionType.WF_ACT_LEAVE_TEAM:
                    handler = new TeamLeave(item.Id);
                    break;
                case InteractionType.WF_ACT_JOIN_TEAM:
                    handler = new TeamJoin(1, item.Id);
                    break;
                #endregion
                #region Condition
                case InteractionType.CONDITION_SUPER_WIRED:
                    handler = new SuperWiredCondition(item, string.Empty, false);
                    break;
                case InteractionType.CONDITION_FURNIS_HAVE_USERS:
                    handler = new FurniHasUser(item, new List<Item>());
                    break;
                case InteractionType.CONDITIONFURNISHAVENOUSERS:
                    handler = new FurniHasNoUser(item, new List<Item>());
                    break;
                case InteractionType.CONDITIONSTATEPOS:
                    handler = new FurniStatePosMatch(item, new List<Item>(), 0, 0, 0);
                    break;
                case InteractionType.WF_CND_STUFF_IS:
                    handler = new FurniStuffIs(item, new List<Item>());
                    break;
                case InteractionType.WF_CND_NOT_STUFF_IS:
                    handler = new FurniNotStuffIs(item, new List<Item>());
                    break;
                case InteractionType.CONDITIONSTATEPOSNEGATIVE:
                    handler = new FurniStatePosMatchNegative(item, new List<Item>(), 0, 0, 0);
                    break;
                case InteractionType.CONDITION_DATE_RNG_ACTIVE:
                    handler = new DateRangeActive(item, new List<int>());
                    break;
                case InteractionType.CONDITIONTIMELESSTHAN:
                    handler = new LessThanTimer(0, room, item);
                    break;
                case InteractionType.CONDITIONTIMEMORETHAN:
                    handler = new MoreThanTimer(0, room, item);
                    break;
                case InteractionType.CONDITIONTRIGGERONFURNI:
                    handler = new TriggerUserIsOnFurni(item, new List<Item>());
                    break;
                case InteractionType.CONDITIONTRIGGERONFURNINEGATIVE:
                    handler = new TriggerUserIsOnFurniNegative(item, new List<Item>());
                    break;
                case InteractionType.CONDITIONHASFURNIONFURNI:
                    handler = new HasFurniOnFurni(item, new List<Item>());
                    break;
                case InteractionType.CONDITIONHASFURNIONFURNINEGATIVE:
                    handler = new HasFurniOnFurniNegative(item, new List<Item>());
                    break;
                case InteractionType.CONDITIONACTORINGROUP:
                    handler = new HasUserInGroup(item);
                    break;
                case InteractionType.CONDITIONNOTINGROUP:
                    handler = new HasUserNotInGroup(item);
                    break;
                case InteractionType.WF_CND_ACTOR_IN_TEAM:
                    handler = new ActorInTeam(item, new List<int>());
                    break;
                case InteractionType.WF_CND_NOT_IN_TEAM:
                    handler = new ActorNotInTeam(item.Id, 1);
                    break;
                case InteractionType.WF_CND_NOT_USER_COUNT:
                    handler = new RoomUserNotCount(item, 1, 1);
                    break;
                case InteractionType.WF_CND_USER_COUNT_IN:
                    handler = new RoomUserCount(item, 1, 1);
                    break;
                    #endregion
            }

            return handler;
        }

        internal static void HandleSaveCondition(Client session, Room room, int itemId, List<int> intParams, string stringParam, List<int> stuffIds, int selectionCode)
        {
            Item item = room.GetRoomItemHandler().GetItem(itemId);
            if (item == null)
            {
                return;
            }

            IWired handler = null;

            switch (item.GetBaseItem().InteractionType)
            {
                #region Condition
                case InteractionType.CONDITION_SUPER_WIRED:
                    handler = new SuperWiredCondition(item, stringParam, session.GetHabbo().HasFuse("fuse_superwired"));
                    break;
                case InteractionType.CONDITION_FURNIS_HAVE_USERS:
                    handler = new FurniHasUser(item, GetItems(stuffIds, room));
                    break;
                case InteractionType.CONDITIONFURNISHAVENOUSERS:
                    handler = new FurniHasNoUser(item, GetItems(stuffIds, room));
                    break;
                case InteractionType.CONDITIONSTATEPOS:
                    handler = new FurniStatePosMatch(item, GetItems(stuffIds, room), (intParams.Count > 0) ? intParams[0] : 0, (intParams.Count > 1) ? intParams[1] : 0, (intParams.Count > 2) ? intParams[2] : 0);
                    break;
                case InteractionType.WF_CND_STUFF_IS:
                    handler = new FurniStuffIs(item, GetItems(stuffIds, room));
                    break;
                case InteractionType.WF_CND_NOT_STUFF_IS:
                    handler = new FurniNotStuffIs(item, GetItems(stuffIds, room));
                    break;
                case InteractionType.CONDITION_DATE_RNG_ACTIVE:
                    handler = new DateRangeActive(item, intParams);
                    break;
                case InteractionType.CONDITIONSTATEPOSNEGATIVE:
                    List<Item> items17 = GetItems(stuffIds, room);
                    handler = new FurniStatePosMatchNegative(item, items17, (intParams.Count > 0) ? intParams[0] : 0, (intParams.Count > 1) ? intParams[1] : 0, (intParams.Count > 2) ? intParams[2] : 0);
                    break;
                case InteractionType.CONDITIONTIMELESSTHAN:
                    handler = new LessThanTimer((intParams.Count > 0) ? intParams[0] : 0, room, item);
                    break;
                case InteractionType.CONDITIONTIMEMORETHAN:
                    handler = new MoreThanTimer((intParams.Count > 0) ? intParams[0] : 0, room, item);
                    break;
                case InteractionType.CONDITIONTRIGGERONFURNI:
                    handler = new TriggerUserIsOnFurni(item, GetItems(stuffIds, room));
                    break;
                case InteractionType.CONDITIONTRIGGERONFURNINEGATIVE:
                    handler = new TriggerUserIsOnFurniNegative(item, GetItems(stuffIds, room));
                    break;
                case InteractionType.CONDITIONHASFURNIONFURNI:
                    handler = new HasFurniOnFurni(item, GetItems(stuffIds, room));
                    break;
                case InteractionType.CONDITIONHASFURNIONFURNINEGATIVE:
                    handler = new HasFurniOnFurniNegative(item, GetItems(stuffIds, room));
                    break;
                case InteractionType.CONDITIONACTORINGROUP:
                    handler = new HasUserInGroup(item);
                    break;
                case InteractionType.WF_CND_NOT_USER_COUNT:
                    handler = new RoomUserNotCount(item, (intParams.Count > 0) ? intParams[0] : 0, (intParams.Count > 1) ? intParams[1] : 0);
                    break;
                case InteractionType.WF_CND_USER_COUNT_IN:
                    handler = new RoomUserCount(item, (intParams.Count > 0) ? intParams[0] : 0, (intParams.Count > 1) ? intParams[1] : 0);
                    break;
                case InteractionType.CONDITIONNOTINGROUP:
                    handler = new HasUserNotInGroup(item);
                    break;
                case InteractionType.WF_CND_ACTOR_IN_TEAM:
                    handler = new ActorInTeam(item, intParams);
                    break;
                case InteractionType.WF_CND_NOT_IN_TEAM:
                    handler = new ActorNotInTeam(item.Id, (intParams.Count > 0) ? intParams[0] : 0);
                    break;
                    #endregion
            }

            if (handler != null)
            {
                HandleSave(handler, room, item);
            }
        }

        internal static void HandleSaveAction(Client session, Room room, int itemId, List<int> intParams, string stringParam, List<int> stuffIds, int delay, int selectionCode)
        {
            Item item = room.GetRoomItemHandler().GetItem(itemId);
            if (item == null)
            {
                return;
            }

            IWired handler = null;

            switch (item.GetBaseItem().InteractionType)
            {
                #region Action
                case InteractionType.ACTIONGIVESCORE:
                    handler = new GiveScore((intParams.Count > 1) ? intParams[1] : 0, (intParams.Count > 0) ? intParams[0] : 0, room.GetGameManager(), itemId);
                    break;
                case InteractionType.WF_ACT_GIVE_SCORE_TM:
                    handler = new GiveScoreTeam((intParams.Count > 2) ? intParams[2] : 0, (intParams.Count > 1) ? intParams[1] : 0, (intParams.Count > 0) ? intParams[0] : 0, room.GetGameManager(), itemId);
                    break;
                case InteractionType.ACTIONPOSRESET:
                    handler = new PositionReset(GetItems(stuffIds, room), delay, room.GetRoomItemHandler(), room.GetWiredHandler(), itemId, (intParams.Count > 0) ? intParams[0] : 0, (intParams.Count > 1) ? intParams[1] : 0, (intParams.Count > 2) ? intParams[2] : 0);

                    break;
                case InteractionType.ACTIONMOVEROTATE:
                    handler = new MoveRotate((intParams.Count > 0) ? intParams[0] : 0, (intParams.Count > 1) ? intParams[1] : 0, GetItems(stuffIds, room), delay, room, room.GetWiredHandler(), itemId);
                    break;
                case InteractionType.ACTIONRESETTIMER:
                    handler = new TimerReset(room, room.GetWiredHandler(), delay, itemId);
                    break;
                case InteractionType.HIGHSCORE:
                case InteractionType.HIGHSCOREPOINTS:

                    break;
                case InteractionType.ACTIONSHOWMESSAGE:
                    handler = new ShowMessage(stringParam, room.GetWiredHandler(), itemId, delay);
                    break;
                case InteractionType.SUPERWIRED:
                    handler = new SuperWired(stringParam, delay, session.GetHabbo().HasFuse("fuse_superwired_god"), session.GetHabbo().HasFuse("fuse_superwired_staff"), room.GetWiredHandler(), itemId);
                    break;
                case InteractionType.ACTIONGIVEREWARD:
                    if (!session.GetHabbo().HasFuse("fuse_superwired"))
                    {
                        return;
                    }
                    //WiredSaver.handler = (IWiredTrigger)new GiveReward(clientMessage.PopString(), room.GetWiredHandler(), itemID), room, roomItem);
                    break;
                case InteractionType.ACTIONKICKUSER:
                    handler = new KickUser(stringParam, room.GetWiredHandler(), itemId, room);
                    break;
                case InteractionType.ACTIONTELEPORTTO:
                    handler = new TeleportToItem(room.GetGameMap(), room.GetWiredHandler(), GetItems(stuffIds, room), delay, itemId);
                    break;
                case InteractionType.WF_ACT_ENDGAME_TEAM:
                    handler = new TeamGameOver((intParams.Count > 0) ? intParams[0] : 0, item.Id, room);
                    break;
                case InteractionType.ACTIONTOGGLESTATE:
                    handler = new ToggleItemState(room.GetGameMap(), room.GetWiredHandler(), GetItems(stuffIds, room), delay, item);
                    break;
                case InteractionType.WF_ACT_CALL_STACKS:
                    handler = new ExecutePile(GetItems(stuffIds, room), delay, room.GetWiredHandler(), item);

                    break;
                case InteractionType.ACTIONFLEE:
                    handler = new Escape(GetItems(stuffIds, room), room, room.GetWiredHandler(), item.Id);
                    break;
                case InteractionType.ACTIONCHASE:
                    handler = new Chase(GetItems(stuffIds, room), room, room.GetWiredHandler(), item.Id);
                    break;
                case InteractionType.COLLISIONTEAM:
                    handler = new CollisionTeam((intParams.Count > 0) ? intParams[0] : 0, room, room.GetWiredHandler(), item.Id);
                    break;
                case InteractionType.COLLISIONCASE:
                    handler = new CollisionCase(GetItems(stuffIds, room), room, room.GetWiredHandler(), item.Id);
                    break;
                case InteractionType.ACTIONMOVETODIR:

                    handler = new MoveToDir(GetItems(stuffIds, room), room, room.GetWiredHandler(), item.Id, (intParams.Count > 0) ? intParams[0] : 0, (intParams.Count > 1) ? intParams[1] : 0);
                    break;
                case InteractionType.WF_ACT_BOT_CLOTHES:
                    handler = new BotClothes(stringParam, room.GetWiredHandler(), item.Id);
                    break;
                case InteractionType.WF_ACT_BOT_TELEPORT:
                    handler = new BotTeleport(stringParam, GetItems(stuffIds, room), room.GetGameMap(), room.GetWiredHandler(), item.Id);
                    break;
                case InteractionType.WF_ACT_BOT_FOLLOW_AVATAR:
                    handler = new BotFollowAvatar(stringParam, (((intParams.Count > 0) ? intParams[0] : 0) == 1), room.GetWiredHandler(), item.Id);
                    break;
                case InteractionType.WF_ACT_BOT_GIVE_HANDITEM:
                    handler = new BotGiveHanditem("", room.GetWiredHandler(), item.Id); //TODO: fix this
                    break;
                case InteractionType.WF_ACT_BOT_MOVE:
                    handler = new BotMove(stringParam, GetItems(stuffIds, room), room.GetWiredHandler(), item.Id);
                    break;
                case InteractionType.WF_ACT_USER_MOVE:
                    handler = new UserMove(GetItems(stuffIds, room), delay, room.GetWiredHandler(), item.Id);
                    break;
                case InteractionType.WF_ACT_BOT_TALK_TO_AVATAR:
                    handler = new BotTalkToAvatar(stringParam, (((intParams.Count > 0) ? intParams[0] : 0) == 1), room.GetWiredHandler(), item.Id);
                    break;
                case InteractionType.WF_ACT_BOT_TALK:
                    handler = new BotTalk(stringParam, (((intParams.Count > 0) ? intParams[0] : 0) == 1), room.GetWiredHandler(), item.Id);
                    break;
                case InteractionType.WF_ACT_LEAVE_TEAM:
                    handler = new TeamLeave(item.Id);
                    break;
                case InteractionType.WF_ACT_JOIN_TEAM:
                    handler = new TeamJoin((intParams.Count > 0) ? intParams[0] : 0, item.Id);
                    break;
               #endregion
            }

            if (handler != null)
            {
                HandleSave(handler, room, item);
            }
        }

        internal static void HandleSaveTrigger(Client session, Room room, int itemId, List<int> intParams, string stringParam, List<int> stuffIds, int selectionCode)
        {
            Item item = room.GetRoomItemHandler().GetItem(itemId);
            if (item == null)
            {
                return;
            }

            IWired handler = null;
            switch (item.GetBaseItem().InteractionType)
            {
                #region Trigger
                case InteractionType.TRIGGER_ONCE:
                    handler = new Timer(item, room.GetWiredHandler(), (intParams.Count > 0) ? intParams[0] : 0, room.GetGameManager());
                    break;
                case InteractionType.TRIGGER_AVATAR_ENTERS_ROOM:
                    handler = new EntersRoom(item, room.GetWiredHandler(), room.GetRoomUserManager(), !string.IsNullOrEmpty(stringParam), stringParam);
                    break;
                case InteractionType.TRIGGER_COLLISION:
                    handler = new Collision(item, room.GetWiredHandler(), room.GetRoomUserManager());
                    break;
                case InteractionType.TRIGGER_GAME_ENDS:
                    handler = new GameEnds(item, room.GetWiredHandler(), room.GetGameManager());
                    break;
                case InteractionType.TRIGGER_GAME_STARTS:
                    handler = new GameStarts(item, room.GetWiredHandler(), room.GetGameManager());
                    break;
                case InteractionType.TRIGGER_PERIODICALLY:
                    handler = new Repeater(room.GetWiredHandler(), item, (intParams.Count > 0) ? intParams[0] : 0);
                    break;
                case InteractionType.TRIGGER_PERIODICALLY_LONG:
                    handler = new Repeaterlong(room.GetWiredHandler(), item, (intParams.Count > 0) ? intParams[0] : 0);
                    break;
                case InteractionType.TRIGGER_AVATAR_SAYS_SOMETHING:
                    handler = new UserSays(item, room.GetWiredHandler(), ((intParams.Count > 0) ? intParams[0] : 0) == 1, stringParam, room);
                    break;
                case InteractionType.TRIGGER_COMMAND:
                    handler = new UserCommand(item, room.GetWiredHandler(), room);
                    break;
                case InteractionType.WIRED_TRIGGER_SELF:
                    handler = new UserTriggerSelf(item, room.GetWiredHandler(), room);
                    break;
                case InteractionType.WF_TRG_BOT_REACHED_AVTR:
                    handler = new BotReadchedAvatar(item, room.GetWiredHandler(), stringParam);
                    break;
                case InteractionType.TRIGGER_COLLISION_USER:
                    handler = new UserCollision(item, room.GetWiredHandler(), room);
                    break;
                case InteractionType.TRIGGER_SCORE_ACHIEVED:
                    handler = new ScoreAchieved(item, room.GetWiredHandler(), (intParams.Count > 0) ? intParams[0] : 0, room.GetGameManager());
                    break;
                case InteractionType.TRIGGER_STATE_CHANGED:
                    handler = new SateChanged(room.GetWiredHandler(), item, GetItems(stuffIds, room), stuffIds, (intParams.Count > 0) ? intParams[0] : 0);
                    break;
                case InteractionType.TRIGGER_WALK_ON_FURNI:
                    handler = new WalksOnFurni(item, room.GetWiredHandler(), GetItems(stuffIds, room), stuffIds, (intParams.Count > 0) ? intParams[0] : 0);
                    break;
                case InteractionType.TRIGGER_WALK_OFF_FURNI:
                    handler = new WalksOffFurni(item, room.GetWiredHandler(), GetItems(stuffIds, room), stuffIds, (intParams.Count > 0) ? intParams[0] : 0);
                    break;
                #endregion
            }

            if (handler != null)
            {
                HandleSave(handler, room, item);
            }
        }

        public static void LoadWiredItem(Item item, Room room, IQueryAdapter dbClient)
        {
            IWired handler = GetWiredHandler(item, room);

            if (handler != null)
            {
                DataRow row = ItemWiredDao.GetOne(dbClient, item.Id);
                if (row != null)
                    handler.LoadFromDatabase(row, room);

                HandleItemLoad(handler, item);
            }
        }

        private static void HandleItemLoad(IWired handler, Item item)
        {
            if (item.WiredHandler != null)
            {
                item.WiredHandler.Dispose();
            }

            item.WiredHandler = handler;
        }

        public static void HandleDefaultSave(Room room, Item item)
        {
            IWired handler = GetWiredHandler(item, room);

            if (handler != null)
            {
                HandleSave(handler, room, item);
            }
        }

        public static void HandleSave(Client session, int itemId, Room room, ClientPacket clientMessage)
        {
            Item item = room.GetRoomItemHandler().GetItem(itemId);
            if (item == null)
            {
                return;
            }

            if (item.WiredHandler != null)
            {
                item.WiredHandler.Dispose();
                item.WiredHandler = null;
            }

            session.SendPacket(new SaveWiredMessageComposer());
        }

        private static List<Item> GetItems(List<int> stuffIds, Room room)
        {
            List<Item> listItem = new List<Item>();
            foreach(int itemId in stuffIds)
            {
                Item item = room.GetRoomItemHandler().GetItem(itemId);
                if (item != null && item.GetBaseItem().Type == 's')
                {
                    listItem.Add(item);
                }
            }

            return listItem;
        }

        private static void HandleSave(IWired handler, Room room, Item item)
        {
            if (item.WiredHandler != null)
            {
                item.WiredHandler.Dispose();
                item.WiredHandler = null;
            }

            item.WiredHandler = handler;
            room.GetWiredHandler().RemoveFurniture(item);
            room.GetWiredHandler().AddFurniture(item);

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                handler.SaveToDatabase(dbClient);
            }
        }
    }
}
