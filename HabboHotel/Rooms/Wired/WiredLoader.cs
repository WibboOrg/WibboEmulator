using Butterfly.Database.Interfaces;
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
    public class WiredLoader
    {
        public static void LoadWiredItem(Item item, Room room, IQueryAdapter dbClient)
        {
            IWired handler = null;
            switch (item.GetBaseItem().InteractionType)
            {
                #region Trigger
                case InteractionType.TRIGGERTIMER:
                    handler = new Timer(item, room.GetWiredHandler(), 2, room.GetGameManager());
                    handler.LoadFromDatabase(dbClient, room);
                    break;
                case InteractionType.TRIGGERROOMENTER:
                    handler = new EntersRoom(item, room.GetWiredHandler(), room.GetRoomUserManager(), false, string.Empty);
                    handler.LoadFromDatabase(dbClient, room);
                    break;
                case InteractionType.TRIGGERCOLLISION:
                    handler = new Collision(item, room.GetWiredHandler(), room.GetRoomUserManager());
                    handler.LoadFromDatabase(dbClient, room);
                    break;
                case InteractionType.TRIGGERGAMEEND:
                    HandleItemLoad(new GameEnds(item, room.GetWiredHandler(), room.GetGameManager()), item);
                    break;
                case InteractionType.TRIGGERGAMESTART:
                    HandleItemLoad(new GameStarts(item, room.GetWiredHandler(), room.GetGameManager()), item);
                    break;
                case InteractionType.TRIGGERREPEATER:
                    handler = new Repeater(room.GetWiredHandler(), item, 0);
                    handler.LoadFromDatabase(dbClient, room);
                    break;
                case InteractionType.TRIGGERREPEATERLONG:
                    handler = new Repeaterlong(room.GetWiredHandler(), item, 0);
                    handler.LoadFromDatabase(dbClient, room);
                    break;
                case InteractionType.TRIGGERONUSERSAY:
                    handler = new UserSays(item, room.GetWiredHandler(), false, string.Empty, room);
                    handler.LoadFromDatabase(dbClient, room);
                    break;
                case InteractionType.TRIGGERCOMMAND:
                    handler = new UserCommand(item, room.GetWiredHandler(), room);
                    handler.LoadFromDatabase(dbClient, room);
                    break;
                case InteractionType.WIRED_TRIGGER_SELF:
                    handler = new UserTriggerSelf(item, room.GetWiredHandler(), room);
                    handler.LoadFromDatabase(dbClient, room);
                    break;
                case InteractionType.WF_TRG_BOT_REACHED_AVTR:
                    handler = new BotReadchedAvatar(item, room.GetWiredHandler(), "");
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.TRIGGERCOLLISIONUSER:
                    handler = new UserCollision(item, room.GetWiredHandler(), room);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.TRIGGERSCOREACHIEVED:
                    handler = new ScoreAchieved(item, room.GetWiredHandler(), 0, room.GetGameManager());
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.TRIGGERSTATECHANGED:
                    handler = new SateChanged(room.GetWiredHandler(), item, new List<Item>(), 0);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.TRIGGERWALKONFURNI:
                    handler = new WalksOnFurni(item, room.GetWiredHandler(), new List<Item>(), 0);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.TRIGGERWALKOFFFURNI:
                    handler = new WalksOffFurni(item, room.GetWiredHandler(), new List<Item>(), 0);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                #endregion
                #region Action
                case InteractionType.ACTIONGIVESCORE:
                    handler = new GiveScore(0, 0, room.GetGameManager(), item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.WF_ACT_GIVE_SCORE_TM:
                    handler = new GiveScoreTeam(0, 0, 0, room.GetGameManager(), item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.ACTIONPOSRESET:
                    handler = new PositionReset(new List<Item>(), 0, room.GetRoomItemHandler(), room.GetWiredHandler(), item.Id, 0, 0, 0);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.ACTIONMOVEROTATE:
                    handler = new MoveRotate(MovementState.none, RotationState.NONE, new List<Item>(), 0, room, room.GetWiredHandler(), item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.ACTIONRESETTIMER:
                    handler = new TimerReset(room, room.GetWiredHandler(), 1, item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.HIGHSCORE:
                    handler = new HighScore(item);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.HIGHSCOREPOINTS:
                    handler = new HighScorePoints(item);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.ACTIONSHOWMESSAGE:
                    handler = new ShowMessage(string.Empty, room.GetWiredHandler(), item.Id, 0);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.ACTIONGIVEREWARD:
                    //handlergr = (IWiredTrigger) new GiveReward(string.Empty, room.GetWiredHandler(), item.Id);
                    //handlergr.LoadFromDatabase(dbClient, room);
                    //WiredLoader.HandleItemLoad(handlergr, item);
                    break;
                case InteractionType.SUPERWIRED:
                    handler = new SuperWired(string.Empty, 0, false, false, room.GetWiredHandler(), item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.ACTIONTELEPORTTO:
                    handler = new TeleportToItem(room.GetGameMap(), room.GetWiredHandler(), new List<Item>(), 0, item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.WF_ACT_ENDGAME_TEAM:
                    handler = new TeamGameOver(1, item.Id, room);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.ACTIONTOGGLESTATE:
                    handler = new ToggleItemState(room.GetGameMap(), room.GetWiredHandler(), new List<Item>(), 0, item);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.WF_ACT_CALL_STACKS:
                    handler = new ExecutePile(new List<Item>(), 0, room.GetWiredHandler(), item);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.ACTIONKICKUSER:
                    handler = new KickUser(string.Empty, room.GetWiredHandler(), item.Id, room);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.ACTIONFLEE:
                    handler = new Escape(new List<Item>(), room, room.GetWiredHandler(), item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.ACTIONCHASE:
                    handler = new Chase(new List<Item>(), room, room.GetWiredHandler(), item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.COLLISIONTEAM:
                    handler = new CollisionTeam(1, room, room.GetWiredHandler(), item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.COLLISIONCASE:
                    handler = new CollisionCase(new List<Item>(), room, room.GetWiredHandler(), item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.ACTIONMOVETODIR:
                    handler = new MoveToDir(new List<Item>(), room, room.GetWiredHandler(), item.Id, MovementDirection.up, WhenMovementBlock.none);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.WF_ACT_BOT_CLOTHES:
                    handler = new BotClothes("", "", room.GetWiredHandler(), item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.WF_ACT_BOT_TELEPORT:
                    handler = new BotTeleport("", new List<Item>(), room.GetGameMap(), room.GetWiredHandler(), item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.WF_ACT_BOT_FOLLOW_AVATAR:
                    handler = new BotFollowAvatar("", false, room.GetWiredHandler(), item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.WF_ACT_BOT_GIVE_HANDITEM:
                    handler = new BotGiveHanditem("", room.GetWiredHandler(), item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.WF_ACT_BOT_MOVE:
                    handler = new BotMove("", new List<Item>(), room.GetWiredHandler(), item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;

                case InteractionType.WF_ACT_USER_MOVE:
                    handler = new UserMove(new List<Item>(), 0, room.GetWiredHandler(), item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.WF_ACT_BOT_TALK_TO_AVATAR:
                    handler = new BotTalkToAvatar("", "", false, room.GetWiredHandler(), item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.WF_ACT_BOT_TALK:
                    handler = new BotTalk("", "", false, room.GetWiredHandler(), item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.WF_ACT_LEAVE_TEAM:
                    handler = new TeamLeave(item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.WF_ACT_JOIN_TEAM:
                    handler = new TeamJoin(1, item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                #endregion
                #region Condition
                case InteractionType.SUPERWIREDCONDITION:
                    handler = new SuperWiredCondition(item, string.Empty, false);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.CONDITIONFURNISHAVEUSERS:
                    handler = new FurniHasUser(item, new List<Item>());
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.CONDITIONFURNISHAVENOUSERS:
                    handler = new FurniHasNoUser(item, new List<Item>());
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.CONDITIONSTATEPOS:
                    handler = new FurniStatePosMatch(item, new List<Item>(), 0, 0, 0);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.WF_CND_STUFF_IS:
                    handler = new FurniStuffIs(item, new List<Item>());
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.WF_CND_NOT_STUFF_IS:
                    handler = new FurniNotStuffIs(item, new List<Item>());
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.CONDITIONSTATEPOSNEGATIVE:
                    handler = new FurniStatePosMatchNegative(item, new List<Item>(), 0, 0, 0);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.CONDITIONTIMELESSTHAN:
                    handler = new LessThanTimer(0, room, item);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.CONDITIONTIMEMORETHAN:
                    handler = new MoreThanTimer(0, room, item);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.CONDITIONTRIGGERONFURNI:
                    handler = new TriggerUserIsOnFurni(item, new List<Item>());
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.CONDITIONTRIGGERONFURNINEGATIVE:
                    handler = new TriggerUserIsOnFurniNegative(item, new List<Item>());
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.CONDITIONHASFURNIONFURNI:
                    handler = new HasFurniOnFurni(item, new List<Item>());
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.CONDITIONHASFURNIONFURNINEGATIVE:
                    handler = new HasFurniOnFurniNegative(item, new List<Item>());
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.CONDITIONACTORINGROUP:
                    handler = new HasUserInGroup(item);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.CONDITIONNOTINGROUP:
                    handler = new HasUserNotInGroup(item);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.WF_CND_ACTOR_IN_TEAM:
                    handler = new ActorInTeam(item.Id, 1);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.WF_CND_NOT_IN_TEAM:
                    handler = new ActorNotInTeam(item.Id, 1);
                    handler.LoadFromDatabase(dbClient, room);
                    break;
                case InteractionType.WF_CND_NOT_USER_COUNT:
                    handler = new RoomUserNotCount(item, 1, 1);
                    handler.LoadFromDatabase(dbClient, room);
                    break;
                case InteractionType.WF_CND_USER_COUNT_IN:
                    handler = new RoomUserCount(item, 1, 1);
                    handler.LoadFromDatabase(dbClient, room);
                    break;
                    #endregion
            }

            if (handler != null)
            {
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
    }
}
