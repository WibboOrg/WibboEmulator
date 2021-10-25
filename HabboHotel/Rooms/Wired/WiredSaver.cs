using Butterfly.Communication.Packets.Incoming;
using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms.Map.Movement;
using Butterfly.HabboHotel.Rooms.Wired.WiredHandlers;
using Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Conditions;
using Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Effects;
using Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Triggers;
using System.Collections.Generic;

namespace Butterfly.HabboHotel.Rooms.Wired
{
    public class WiredSaver
    {
        public static void HandleDefaultSave(int itemID, Room room, Item roomItem)
        {
            if (roomItem == null)
            {
                return;
            }

            switch (roomItem.GetBaseItem().InteractionType)
            {
                #region SaveDefaultTrigger
                case InteractionType.TRIGGERTIMER:
                    int cycleCount = 0;
                    HandleTriggerSave(new Timer(roomItem, room.GetWiredHandler(), cycleCount, room.GetGameManager()), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.TRIGGERROOMENTER:
                    string userName = string.Empty;
                    HandleTriggerSave(new EntersRoom(roomItem, room.GetWiredHandler(), room.GetRoomUserManager(), !string.IsNullOrEmpty(userName), userName), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.TRIGGERCOLLISION:
                    HandleTriggerSave(new Collision(roomItem, room.GetWiredHandler(), room.GetRoomUserManager()), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.TRIGGERGAMEEND:
                    HandleTriggerSave(new GameEnds(roomItem, room.GetWiredHandler(), room.GetGameManager()), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.TRIGGERGAMESTART:
                    HandleTriggerSave(new GameStarts(roomItem, room.GetWiredHandler(), room.GetGameManager()), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.TRIGGERREPEATER:
                    int cyclesRequired = 0;
                    HandleTriggerSave(new Repeater(room.GetWiredHandler(), roomItem, cyclesRequired), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.TRIGGERREPEATERLONG:
                    int cyclesRequiredlong = 0;
                    HandleTriggerSave(new Repeaterlong(room.GetWiredHandler(), roomItem, cyclesRequiredlong), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.TRIGGERONUSERSAY:
                    bool flag = false;
                    string triggerMessage = string.Empty;
                    HandleTriggerSave(new UserSays(roomItem, room.GetWiredHandler(), !flag, triggerMessage, room), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.TRIGGERCOMMAND:
                    HandleTriggerSave(new UserCommand(roomItem, room.GetWiredHandler(), room), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WIRED_TRIGGER_SELF:
                    HandleTriggerSave(new UserTriggerSelf(roomItem, room.GetWiredHandler(), room), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_TRG_BOT_REACHED_AVTR:
                    HandleTriggerSave(new BotReadchedAvatar(roomItem, room.GetWiredHandler(), ""), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.TRIGGERCOLLISIONUSER:
                    HandleTriggerSave(new UserCollision(roomItem, room.GetWiredHandler(), room), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.TRIGGERSCOREACHIEVED:
                    int scoreLevel = 0;
                    HandleTriggerSave(new ScoreAchieved(roomItem, room.GetWiredHandler(), scoreLevel, room.GetGameManager()), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.TRIGGERSTATECHANGED:
                    List<Item> items1 = new List<Item>();
                    int delay1 = 0;
                    HandleTriggerSave(new SateChanged(room.GetWiredHandler(), roomItem, items1, delay1), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.TRIGGERWALKONFURNI:
                    List<Item> targetItems1 = new List<Item>();
                    int requiredCycles1 = 0;
                    HandleTriggerSave(new WalksOnFurni(roomItem, room.GetWiredHandler(), targetItems1, requiredCycles1), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.TRIGGERWALKOFFFURNI:
                    List<Item> targetItems2 = new List<Item>();
                    int requiredCycles2 = 0;
                    HandleTriggerSave(new WalksOffFurni(roomItem, room.GetWiredHandler(), targetItems2, requiredCycles2), room.GetWiredHandler(), room, roomItem);
                    break;
                #endregion
                #region SauveDefaultAction
                case InteractionType.ACTIONGIVESCORE:
                    HandleTriggerSave(new GiveScore(0, 0, room.GetGameManager(), itemID), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_ACT_GIVE_SCORE_TM:
                    HandleTriggerSave(new GiveScoreTeam(0, 0, 0, room.GetGameManager(), itemID), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.ACTIONPOSRESET:
                    HandleTriggerSave(new PositionReset(new List<Item>(), 0, room.GetRoomItemHandler(), room.GetWiredHandler(), itemID, 0, 0, 0), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.ACTIONMOVEROTATE:
                    HandleTriggerSave(new MoveRotate(MovementState.none, RotationState.NONE, new List<Item>(), 0, room, room.GetWiredHandler(), itemID), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.ACTIONRESETTIMER:
                    int delay2 = 0;
                    HandleTriggerSave(new TimerReset(room, room.GetWiredHandler(), delay2, itemID), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.HIGHSCORE:
                    HandleTriggerSave(new HighScore(roomItem), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.HIGHSCOREPOINTS:
                    HandleTriggerSave(new HighScorePoints(roomItem), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.ACTIONSHOWMESSAGE:
                    HandleTriggerSave(new ShowMessage(string.Empty, room.GetWiredHandler(), itemID, 0), room.GetWiredHandler(), room, roomItem);
                    break;
                //case InteractionType.actiongivereward:
                //WiredSaver.HandleTriggerSave(new GiveReward(string.Empty, room.GetWiredHandler(), itemID), room.GetWiredHandler(), room, roomItem);
                //break;
                case InteractionType.SUPERWIRED:
                    HandleTriggerSave(new SuperWired(string.Empty, 0, false, false, room.GetWiredHandler(), itemID), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.ACTIONKICKUSER:
                    HandleTriggerSave(new KickUser(string.Empty, room.GetWiredHandler(), itemID, room), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.ACTIONTELEPORTTO:
                    List<Item> items3 = new List<Item>();
                    int delay3 = 0;
                    HandleTriggerSave(new TeleportToItem(room.GetGameMap(), room.GetWiredHandler(), items3, delay3, itemID), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_ACT_ENDGAME_TEAM:
                    List<Item> itemstpteam = new List<Item>();
                    HandleTriggerSave(new TeamGameOver(1, itemID, room), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.ACTIONTOGGLESTATE:
                    List<Item> items4 = new List<Item>();
                    int delay4 = 0;
                    HandleTriggerSave(new ToggleItemState(room.GetGameMap(), room.GetWiredHandler(), items4, delay4, roomItem), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_ACT_CALL_STACKS:
                    HandleTriggerSave(new ExecutePile(new List<Item>(), 0, room.GetWiredHandler(), roomItem), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.ACTIONFLEE:
                    List<Item> itemsflee = new List<Item>();
                    HandleTriggerSave(new Escape(itemsflee, room, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.ACTIONCHASE:
                    List<Item> itemschase = new List<Item>();
                    HandleTriggerSave(new Chase(itemschase, room, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.COLLISIONTEAM:
                    HandleTriggerSave(new CollisionTeam(1, room, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.COLLISIONCASE:
                    List<Item> itemscollision = new List<Item>();
                    HandleTriggerSave(new CollisionCase(itemscollision, room, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.ACTIONMOVETODIR:
                    HandleTriggerSave(new MoveToDir(new List<Item>(), room, room.GetWiredHandler(), roomItem.Id, MovementDirection.none, WhenMovementBlock.none), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_ACT_BOT_CLOTHES:
                    HandleTriggerSave(new BotClothes("", "", room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_ACT_BOT_TELEPORT:
                    HandleTriggerSave(new BotTeleport("", new List<Item>(), room.GetGameMap(), room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_ACT_BOT_FOLLOW_AVATAR:
                    HandleTriggerSave(new BotFollowAvatar("", false, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_ACT_BOT_GIVE_HANDITEM:
                    HandleTriggerSave(new BotGiveHanditem("", room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_ACT_BOT_MOVE:
                    HandleTriggerSave(new BotMove("", new List<Item>(), room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_ACT_USER_MOVE:
                    HandleTriggerSave(new UserMove(new List<Item>(), 0, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_ACT_BOT_TALK_TO_AVATAR:
                    HandleTriggerSave(new BotTalkToAvatar("", "", false, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_ACT_BOT_TALK:
                    HandleTriggerSave(new BotTalk("", "", false, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_ACT_LEAVE_TEAM:
                    HandleTriggerSave(new TeamLeave(itemID), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_ACT_JOIN_TEAM:
                    HandleTriggerSave(new TeamJoin(1, itemID), room.GetWiredHandler(), room, roomItem);
                    break;
                #endregion
                #region SaveDefaultCondition
                case InteractionType.SUPERWIREDCONDITION:
                    HandleTriggerSave(new SuperWiredCondition(roomItem, "", false), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.CONDITIONFURNISHAVEUSERS:
                    HandleTriggerSave(new FurniHasUser(roomItem, new List<Item>()), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.CONDITIONFURNISHAVENOUSERS:
                    HandleTriggerSave(new FurniHasNoUser(roomItem, new List<Item>()), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.CONDITIONSTATEPOS:
                    HandleTriggerSave(new FurniStatePosMatch(roomItem, new List<Item>(), 0, 0, 0), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_CND_STUFF_IS:
                    HandleTriggerSave(new FurniStuffIs(roomItem, new List<Item>()), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_CND_NOT_STUFF_IS:
                    HandleTriggerSave(new FurniNotStuffIs(roomItem, new List<Item>()), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.CONDITIONSTATEPOSNEGATIVE:
                    HandleTriggerSave(new FurniStatePosMatchNegative(roomItem, new List<Item>(), 0, 0, 0), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.CONDITIONTIMELESSTHAN:
                    HandleTriggerSave(new LessThanTimer(0, room, roomItem), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.CONDITIONTIMEMORETHAN:
                    List<Item> items8 = new List<Item>();
                    HandleTriggerSave(new MoreThanTimer(0, room, roomItem), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.CONDITIONTRIGGERONFURNI:
                    List<Item> items9 = new List<Item>();
                    HandleTriggerSave(new TriggerUserIsOnFurni(roomItem, items9), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.CONDITIONTRIGGERONFURNINEGATIVE:
                    List<Item> items12 = new List<Item>();
                    HandleTriggerSave(new TriggerUserIsOnFurniNegative(roomItem, items12), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.CONDITIONHASFURNIONFURNI:
                    List<Item> items11 = new List<Item>();
                    HandleTriggerSave(new HasFurniOnFurni(roomItem, items11), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.CONDITIONHASFURNIONFURNINEGATIVE:
                    List<Item> items14 = new List<Item>();
                    HandleTriggerSave(new HasFurniOnFurniNegative(roomItem, items14), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.CONDITIONACTORINGROUP:
                    HandleTriggerSave(new HasUserInGroup(roomItem), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.CONDITIONNOTINGROUP:
                    HandleTriggerSave(new HasUserNotInGroup(roomItem), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_CND_ACTOR_IN_TEAM:
                    HandleTriggerSave(new ActorInTeam(roomItem.Id, 1), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_CND_NOT_IN_TEAM:
                    HandleTriggerSave(new ActorNotInTeam(roomItem.Id, 1), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_CND_NOT_USER_COUNT:
                    HandleTriggerSave(new RoomUserNotCount(roomItem, 1, 1), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_CND_USER_COUNT_IN:
                    HandleTriggerSave(new RoomUserCount(roomItem, 1, 1), room.GetWiredHandler(), room, roomItem);
                    break;
                    #endregion
            }
        }

        public static void HandleSave(GameClient Session, int itemID, Room room, ClientPacket clientMessage)
        {
            Item roomItem = room.GetRoomItemHandler().GetItem(itemID);
            if (roomItem == null)
            {
                return;
            }

            if (roomItem.WiredHandler != null)
            {
                roomItem.WiredHandler.Dispose();
                roomItem.WiredHandler = null;
            }

            switch (roomItem.GetBaseItem().InteractionType)
            {
                #region Trigger
                case InteractionType.TRIGGERTIMER:
                    clientMessage.PopInt();
                    int cycleCount = clientMessage.PopInt();
                    HandleTriggerSave(new Timer(roomItem, room.GetWiredHandler(), cycleCount, room.GetGameManager()), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.TRIGGERROOMENTER:
                    clientMessage.PopInt();
                    string userName = clientMessage.PopString();
                    HandleTriggerSave(new EntersRoom(roomItem, room.GetWiredHandler(), room.GetRoomUserManager(), !string.IsNullOrEmpty(userName), userName), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.TRIGGERCOLLISION:
                    HandleTriggerSave(new Collision(roomItem, room.GetWiredHandler(), room.GetRoomUserManager()), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.TRIGGERGAMEEND:
                    HandleTriggerSave(new GameEnds(roomItem, room.GetWiredHandler(), room.GetGameManager()), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.TRIGGERGAMESTART:
                    HandleTriggerSave(new GameStarts(roomItem, room.GetWiredHandler(), room.GetGameManager()), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.TRIGGERREPEATER:
                    clientMessage.PopInt();
                    int cyclesRequired = clientMessage.PopInt();
                    HandleTriggerSave(new Repeater(room.GetWiredHandler(), roomItem, cyclesRequired), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.TRIGGERREPEATERLONG:
                    clientMessage.PopInt();
                    int cyclesRequiredlong = clientMessage.PopInt();
                    HandleTriggerSave(new Repeaterlong(room.GetWiredHandler(), roomItem, cyclesRequiredlong), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.TRIGGERONUSERSAY:
                    clientMessage.PopInt();
                    bool isOwnerOnly = clientMessage.PopInt() == 1;
                    string triggerMessage = clientMessage.PopString();
                    HandleTriggerSave(new UserSays(roomItem, room.GetWiredHandler(), isOwnerOnly, triggerMessage, room), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.TRIGGERCOMMAND:
                    HandleTriggerSave(new UserCommand(roomItem, room.GetWiredHandler(), room), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WIRED_TRIGGER_SELF:
                    HandleTriggerSave(new UserTriggerSelf(roomItem, room.GetWiredHandler(), room), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_TRG_BOT_REACHED_AVTR:
                    clientMessage.PopInt();

                    string NameBotReached = clientMessage.PopString();
                    HandleTriggerSave(new BotReadchedAvatar(roomItem, room.GetWiredHandler(), NameBotReached), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.TRIGGERCOLLISIONUSER:
                    HandleTriggerSave(new UserCollision(roomItem, room.GetWiredHandler(), room), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.TRIGGERSCOREACHIEVED:
                    clientMessage.PopInt();
                    int scoreLevel = clientMessage.PopInt();
                    HandleTriggerSave(new ScoreAchieved(roomItem, room.GetWiredHandler(), scoreLevel, room.GetGameManager()), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.TRIGGERSTATECHANGED:
                    clientMessage.PopInt();
                    clientMessage.PopBoolean();
                    clientMessage.PopBoolean();
                    List<Item> items1 = GetItems(clientMessage, room, itemID);
                    int delay1 = clientMessage.PopInt();
                    HandleTriggerSave(new SateChanged(room.GetWiredHandler(), roomItem, items1, delay1), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.TRIGGERWALKONFURNI:
                    clientMessage.PopInt();
                    clientMessage.PopString();
                    List<Item> items2 = GetItems(clientMessage, room, itemID);
                    int requiredCycles1 = clientMessage.PopInt();
                    HandleTriggerSave(new WalksOnFurni(roomItem, room.GetWiredHandler(), items2, requiredCycles1), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.TRIGGERWALKOFFFURNI:
                    clientMessage.PopInt();
                    clientMessage.PopString();
                    List<Item> items3 = GetItems(clientMessage, room, itemID);
                    int requiredCycles2 = clientMessage.PopInt();
                    HandleTriggerSave(new WalksOffFurni(roomItem, room.GetWiredHandler(), items3, requiredCycles2), room.GetWiredHandler(), room, roomItem);
                    break;
                #endregion
                #region Action
                case InteractionType.ACTIONGIVESCORE:
                    clientMessage.PopInt();
                    int scoreToGive = clientMessage.PopInt();
                    int maxCountPerGame = clientMessage.PopInt();
                    HandleTriggerSave(new GiveScore(maxCountPerGame, scoreToGive, room.GetGameManager(), itemID), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_ACT_GIVE_SCORE_TM:
                    clientMessage.PopInt();
                    int scoreToGive2 = clientMessage.PopInt();
                    int maxCountPerGame2 = clientMessage.PopInt();
                    int TeamId = clientMessage.PopInt();
                    HandleTriggerSave(new GiveScoreTeam(TeamId, maxCountPerGame2, scoreToGive2, room.GetGameManager(), itemID), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.ACTIONPOSRESET:
                    clientMessage.PopInt();
                    int EtatActuel = clientMessage.PopInt();
                    int DirectionActuel = clientMessage.PopInt();
                    int PositionActuel = clientMessage.PopInt();
                    clientMessage.PopBoolean();
                    clientMessage.PopBoolean();

                    List<Item> itemsposrest = GetItems(clientMessage, room, itemID);
                    int requiredCyclesposrest = clientMessage.PopInt();
                    HandleTriggerSave(new PositionReset(itemsposrest, requiredCyclesposrest, room.GetRoomItemHandler(), room.GetWiredHandler(), itemID, EtatActuel, DirectionActuel, PositionActuel), room.GetWiredHandler(), room, roomItem);

                    break;
                case InteractionType.ACTIONMOVEROTATE:
                    clientMessage.PopInt();
                    MovementState movement = (MovementState)clientMessage.PopInt();
                    RotationState rotation = (RotationState)clientMessage.PopInt();
                    clientMessage.PopBoolean();
                    clientMessage.PopBoolean();
                    List<Item> items4 = GetItems(clientMessage, room, itemID);
                    int delay2 = clientMessage.PopInt();
                    HandleTriggerSave(new MoveRotate(movement, rotation, items4, delay2, room, room.GetWiredHandler(), itemID), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.ACTIONRESETTIMER:
                    clientMessage.PopInt();
                    clientMessage.PopInt();
                    clientMessage.PopString();
                    int delay3 = clientMessage.PopInt();
                    HandleTriggerSave(new TimerReset(room, room.GetWiredHandler(), delay3, itemID), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.HIGHSCORE:
                case InteractionType.HIGHSCOREPOINTS:

                    break;
                case InteractionType.ACTIONSHOWMESSAGE:
                    clientMessage.PopInt();
                    string MessageWired = clientMessage.PopString();
                    int CountItemMessage = clientMessage.PopInt();
                    int DelayMessage = clientMessage.PopInt();
                    HandleTriggerSave(new ShowMessage(MessageWired, room.GetWiredHandler(), itemID, DelayMessage), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.SUPERWIRED:
                    clientMessage.PopInt();
                    string MessageSuperWired = clientMessage.PopString();
                    int CountItemSuperWired = clientMessage.PopInt();
                    int DelaySuperWired = clientMessage.PopInt();
                    HandleTriggerSave(new SuperWired(MessageSuperWired, DelaySuperWired, Session.GetHabbo().HasFuse("fuse_superwired_god"), Session.GetHabbo().HasFuse("fuse_superwired_staff"), room.GetWiredHandler(), itemID), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.ACTIONGIVEREWARD:
                    if (!Session.GetHabbo().HasFuse("fuse_superwired"))
                    {
                        return;
                    }
                    //clientMessage.PopInt();
                    //WiredSaver.HandleTriggerSave((IWiredTrigger)new GiveReward(clientMessage.PopString(), room.GetWiredHandler(), itemID), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.ACTIONKICKUSER:
                    clientMessage.PopInt();
                    HandleTriggerSave(new KickUser(clientMessage.PopString(), room.GetWiredHandler(), itemID, room), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.ACTIONTELEPORTTO:
                    clientMessage.PopInt();
                    clientMessage.PopString();
                    List<Item> items6 = GetItems(clientMessage, room, itemID);
                    int delay4 = clientMessage.PopInt();
                    HandleTriggerSave(new TeleportToItem(room.GetGameMap(), room.GetWiredHandler(), items6, delay4, itemID), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_ACT_ENDGAME_TEAM:
                    clientMessage.PopInt();
                    int TeamId3 = clientMessage.PopInt();
                    HandleTriggerSave(new TeamGameOver(TeamId3, roomItem.Id, room), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.ACTIONTOGGLESTATE:
                    clientMessage.PopInt();
                    clientMessage.PopString();
                    List<Item> items7 = GetItems(clientMessage, room, itemID);
                    int delay5 = clientMessage.PopInt();
                    HandleTriggerSave(new ToggleItemState(room.GetGameMap(), room.GetWiredHandler(), items7, delay5, roomItem), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_ACT_CALL_STACKS:
                    clientMessage.PopInt();
                    clientMessage.PopString();
                    List<Item> itemsExecute = GetItems(clientMessage, room, itemID);
                    int StackDeley = clientMessage.PopInt();
                    HandleTriggerSave(new ExecutePile(itemsExecute, StackDeley, room.GetWiredHandler(), roomItem), room.GetWiredHandler(), room, roomItem);

                    break;
                case InteractionType.ACTIONFLEE:
                    clientMessage.PopInt();
                    clientMessage.PopString();
                    List<Item> itemsflee = GetItems(clientMessage, room, itemID);
                    HandleTriggerSave(new Escape(itemsflee, room, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.ACTIONCHASE:
                    clientMessage.PopInt();
                    clientMessage.PopString();
                    List<Item> itemschase = GetItems(clientMessage, room, itemID);
                    HandleTriggerSave(new Chase(itemschase, room, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.COLLISIONTEAM:
                    clientMessage.PopInt();
                    int TeamIdCollision = clientMessage.PopInt();
                    HandleTriggerSave(new CollisionTeam(TeamIdCollision, room, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.COLLISIONCASE:
                    clientMessage.PopInt();
                    clientMessage.PopString();
                    List<Item> itemscollision = GetItems(clientMessage, room, itemID);
                    HandleTriggerSave(new CollisionCase(itemscollision, room, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.ACTIONMOVETODIR:
                    clientMessage.PopInt();

                    MovementDirection StarDirect = (MovementDirection)clientMessage.PopInt();
                    WhenMovementBlock WhenBlock = (WhenMovementBlock)clientMessage.PopInt();

                    clientMessage.PopBoolean();
                    clientMessage.PopBoolean();

                    List<Item> itemsmovetodir = GetItems(clientMessage, room, itemID);
                    int delaymovetodir = clientMessage.PopInt();

                    HandleTriggerSave(new MoveToDir(itemsmovetodir, room, room.GetWiredHandler(), roomItem.Id, StarDirect, WhenBlock), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_ACT_BOT_CLOTHES:
                    clientMessage.PopInt();

                    string NameAndLook = clientMessage.PopString();

                    string[] SplieNAL = NameAndLook.Split('\t');
                    if (SplieNAL.Length != 2)
                    {
                        break;
                    }

                    HandleTriggerSave(new BotClothes(SplieNAL[0], SplieNAL[1], room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_ACT_BOT_TELEPORT:
                    clientMessage.PopInt();

                    string NameBot = clientMessage.PopString();

                    List<Item> itemsbotteleport = GetItems(clientMessage, room, itemID);

                    HandleTriggerSave(new BotTeleport(NameBot, itemsbotteleport, room.GetGameMap(), room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_ACT_BOT_FOLLOW_AVATAR:
                    clientMessage.PopInt();

                    bool IsFollow = (clientMessage.PopInt() == 1);
                    string NameBotFollow = clientMessage.PopString();

                    HandleTriggerSave(new BotFollowAvatar(NameBotFollow, IsFollow, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_ACT_BOT_GIVE_HANDITEM:
                    clientMessage.PopInt();

                    HandleTriggerSave(new BotGiveHanditem("", room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_ACT_BOT_MOVE:
                    clientMessage.PopInt();

                    string NameBotMove = clientMessage.PopString();

                    List<Item> itemsbotMove = GetItems(clientMessage, room, itemID);

                    HandleTriggerSave(new BotMove(NameBotMove, itemsbotMove, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_ACT_USER_MOVE:
                    clientMessage.PopInt();
                    clientMessage.PopString();
                    List<Item> itemsUserMove = GetItems(clientMessage, room, itemID);

                    int delayusermove = clientMessage.PopInt();
                    HandleTriggerSave(new UserMove(itemsUserMove, delayusermove, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_ACT_BOT_TALK_TO_AVATAR:
                    clientMessage.PopInt();
                    bool IsCrier = (clientMessage.PopInt() == 1);

                    string BotNameAndMessage = clientMessage.PopString();

                    string[] SplieNAM = BotNameAndMessage.Split('\t');
                    if (SplieNAM.Length != 2)
                    {
                        break;
                    }

                    string NameBotTalk = SplieNAM[0];
                    string MessageBot = SplieNAM[1];


                    HandleTriggerSave(new BotTalkToAvatar(NameBotTalk, MessageBot, IsCrier, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_ACT_BOT_TALK:
                    clientMessage.PopInt();
                    bool IsCrier2 = (clientMessage.PopInt() == 1);

                    string BotNameAndMessage2 = clientMessage.PopString();

                    string[] SplieNAM2 = BotNameAndMessage2.Split('\t');
                    if (SplieNAM2.Length != 2)
                    {
                        break;
                    }

                    string NameBotTalk2 = SplieNAM2[0];
                    string MessageBot2 = SplieNAM2[1];


                    HandleTriggerSave(new BotTalk(NameBotTalk2, MessageBot2, IsCrier2, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_ACT_LEAVE_TEAM:
                    HandleTriggerSave(new TeamLeave(roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_ACT_JOIN_TEAM:
                    clientMessage.PopInt();
                    int TeamId4 = clientMessage.PopInt();
                    HandleTriggerSave(new TeamJoin(TeamId4, roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                #endregion
                #region Condition
                case InteractionType.SUPERWIREDCONDITION:
                    clientMessage.PopInt();
                    HandleTriggerSave(new SuperWiredCondition(roomItem, clientMessage.PopString(), Session.GetHabbo().HasFuse("fuse_superwired")), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.CONDITIONFURNISHAVEUSERS:
                    clientMessage.PopInt();
                    clientMessage.PopString();

                    List<Item> items10 = GetItems(clientMessage, room, itemID);
                    HandleTriggerSave(new FurniHasUser(roomItem, items10), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.CONDITIONFURNISHAVENOUSERS:
                    clientMessage.PopInt();
                    clientMessage.PopString();

                    List<Item> items12 = GetItems(clientMessage, room, itemID);
                    HandleTriggerSave(new FurniHasNoUser(roomItem, items12), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.CONDITIONSTATEPOS:
                    clientMessage.PopInt();
                    int EtatActuel2 = clientMessage.PopInt();
                    int DirectionActuel2 = clientMessage.PopInt();
                    int PositionActuel2 = clientMessage.PopInt();
                    clientMessage.PopBoolean();
                    clientMessage.PopBoolean();

                    HandleTriggerSave(new FurniStatePosMatch(roomItem, GetItems(clientMessage, room, itemID), EtatActuel2, DirectionActuel2, PositionActuel2), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_CND_STUFF_IS:
                    clientMessage.PopInt();
                    clientMessage.PopString();

                    HandleTriggerSave(new FurniStuffIs(roomItem, GetItems(clientMessage, room, itemID)), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_CND_NOT_STUFF_IS:
                    clientMessage.PopInt();
                    clientMessage.PopString();

                    HandleTriggerSave(new FurniNotStuffIs(roomItem, GetItems(clientMessage, room, itemID)), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.CONDITIONSTATEPOSNEGATIVE:
                    clientMessage.PopInt();
                    int EtatActuel3 = clientMessage.PopInt();
                    int DirectionActuel3 = clientMessage.PopInt();
                    int PositionActuel3 = clientMessage.PopInt();
                    clientMessage.PopBoolean();
                    clientMessage.PopBoolean();

                    List<Item> items17 = GetItems(clientMessage, room, itemID);
                    HandleTriggerSave(new FurniStatePosMatchNegative(roomItem, items17, EtatActuel3, DirectionActuel3, PositionActuel3), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.CONDITIONTIMELESSTHAN:
                    clientMessage.PopInt();
                    int cycleCount2 = clientMessage.PopInt();
                    HandleTriggerSave(new LessThanTimer(cycleCount2, room, roomItem), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.CONDITIONTIMEMORETHAN:
                    clientMessage.PopInt();
                    int cycleCount3 = clientMessage.PopInt();
                    HandleTriggerSave(new MoreThanTimer(cycleCount3, room, roomItem), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.CONDITIONTRIGGERONFURNI:
                    clientMessage.PopInt();
                    clientMessage.PopString();

                    List<Item> items9 = GetItems(clientMessage, room, itemID);
                    HandleTriggerSave(new TriggerUserIsOnFurni(roomItem, items9), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.CONDITIONTRIGGERONFURNINEGATIVE:
                    clientMessage.PopInt();
                    clientMessage.PopString();

                    List<Item> items14 = GetItems(clientMessage, room, itemID);
                    HandleTriggerSave(new TriggerUserIsOnFurniNegative(roomItem, items14), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.CONDITIONHASFURNIONFURNI:
                    clientMessage.PopInt();
                    clientMessage.PopString();

                    List<Item> items13 = GetItems(clientMessage, room, itemID);
                    HandleTriggerSave(new HasFurniOnFurni(roomItem, items13), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.CONDITIONHASFURNIONFURNINEGATIVE:
                    clientMessage.PopInt();
                    clientMessage.PopString();

                    List<Item> items15 = GetItems(clientMessage, room, itemID);
                    HandleTriggerSave(new HasFurniOnFurniNegative(roomItem, items15), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.CONDITIONACTORINGROUP:
                    HandleTriggerSave(new HasUserInGroup(roomItem), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_CND_NOT_USER_COUNT:
                    clientMessage.PopInt();
                    int UserCountMin = clientMessage.PopInt();
                    int UserCountMax = clientMessage.PopInt();
                    HandleTriggerSave(new RoomUserNotCount(roomItem, UserCountMin, UserCountMax), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_CND_USER_COUNT_IN:
                    clientMessage.PopInt();
                    int UserCountMin2 = clientMessage.PopInt();
                    int UserCountMax2 = clientMessage.PopInt();
                    HandleTriggerSave(new RoomUserCount(roomItem, UserCountMin2, UserCountMax2), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.CONDITIONNOTINGROUP:
                    HandleTriggerSave(new HasUserNotInGroup(roomItem), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_CND_ACTOR_IN_TEAM:
                    clientMessage.PopInt();
                    int TeamId5 = clientMessage.PopInt();
                    HandleTriggerSave(new ActorInTeam(roomItem.Id, TeamId5), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.WF_CND_NOT_IN_TEAM:
                    clientMessage.PopInt();
                    int TeamId2 = clientMessage.PopInt();
                    HandleTriggerSave(new ActorNotInTeam(roomItem.Id, TeamId2), room.GetWiredHandler(), room, roomItem);
                    break;
                    #endregion
            }

            Session.SendPacket(new ServerPacket(ServerPacketHeader.WIRED_SAVE));
        }

        private static List<Item> GetItems(ClientPacket message, Room room, int itemID)
        {
            List<Item> list = new List<Item>();
            int itemCount = message.PopInt();
            for (int index = 0; index < itemCount; ++index)
            {
                Item roomItem = room.GetRoomItemHandler().GetItem(message.PopInt());
                if (roomItem != null && index < 20 && roomItem.GetBaseItem().Type == 's')
                {
                    list.Add(roomItem);
                }
            }

            return list;
        }

        private static void HandleTriggerSave(IWired handler, WiredHandler manager, Room room, Item roomItem)
        {
            if (roomItem == null)
            {
                return;
            }

            roomItem.WiredHandler = handler;
            manager.RemoveFurniture(roomItem);
            manager.AddFurniture(roomItem);

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                handler.SaveToDatabase(dbClient);
            }
        }
    }
}
