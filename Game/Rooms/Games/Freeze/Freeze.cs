using Butterfly.Communication.Packets.Outgoing.GameCenter;
using Butterfly.Communication.Packets.Outgoing.Rooms.Avatar;
using Butterfly.Communication.Packets.Outgoing.Rooms.Freeze;
using Butterfly.Game.Items;
using System.Collections;
using System.Drawing;

namespace Butterfly.Game.Rooms.Games
{
    public class Freeze
    {
        private Room _roomInstance;
        private readonly Dictionary<int, Item> _freezeBlocks;
        private bool _gameStarted;

        public Freeze(Room room)
        {
            this._roomInstance = room;
            this._freezeBlocks = new Dictionary<int, Item>();
            this._gameStarted = false;
        }

        public bool IsGameActive => this._gameStarted;

        public void StartGame()
        {
            if (this._gameStarted)
            {
                return;
            }

            this._gameStarted = true;
            this.CountTeamPoints();
            this.ResetGame();
        }

        public void StopGame()
        {
            if (!this._gameStarted)
            {
                return;
            }

            this._gameStarted = false;

            TeamType winningTeam = this._roomInstance.GetGameManager().GetWinningTeam();

            foreach (RoomUser user in this._roomInstance.GetTeamManager().GetAllPlayer())
            {
                this.EndGame(user, winningTeam);
            }
        }

        private void EndGame(RoomUser roomUser, TeamType winningTeam)
        {
            if (roomUser.Team == winningTeam && winningTeam != TeamType.NONE)
            {
                this._roomInstance.SendPacket(new ActionComposer(roomUser.VirtualId, 1));
            }
            else if (roomUser.Team != TeamType.NONE)
            {
                Item FirstTile = this.GetFirstTile(roomUser.X, roomUser.Y);

                if (FirstTile == null)
                {
                    return;
                }
                roomUser.ApplyEffect(0);
                TeamManager managerForFreeze = this._roomInstance.GetTeamManager();
                managerForFreeze.OnUserLeave(roomUser);

                roomUser.GetClient().SendPacket(new IsPlayingComposer(false));

                this._roomInstance.GetGameManager().UpdateGatesTeamCounts();

                roomUser.Team = TeamType.NONE;

                if (this._roomInstance.GetGameItemHandler().GetExitTeleport() != null)
                {
                    this._roomInstance.GetGameMap().TeleportToItem(roomUser, this._roomInstance.GetGameItemHandler().GetExitTeleport());
                }

                roomUser.Freezed = false;
                roomUser.SetStep = false;
                roomUser.IsWalking = false;
                roomUser.UpdateNeeded = true;
                roomUser.FreezeLives = 0;
            }
        }

        public void CycleUser(RoomUser user)
        {
            if (user.Freezed)
            {
                user.FreezeCounter++;
                if (user.FreezeCounter > 10)
                {
                    user.Freezed = false;
                    user.FreezeCounter = 0;
                    ActivateShield(user);
                }
            }
            if (!user.ShieldActive)
            {
                return;
            }

            ++user.ShieldCounter;
            if (user.ShieldCounter <= 10)
            {
                return;
            }

            user.ShieldActive = false;
            user.ShieldCounter = 10;
            user.ApplyEffect(((int)user.Team + 39));
        }

        public void ResetGame()
        {
            foreach (Item roomItem in (IEnumerable)this._freezeBlocks.Values)
            {
                if (!string.IsNullOrEmpty(roomItem.ExtraData))
                {
                    roomItem.ExtraData = "";
                    roomItem.UpdateState(false, true);
                    this._roomInstance.GetGameMap().UpdateMapForItem(roomItem);
                }
            }
        }

        public void OnWalkFreezeBlock(Item roomitem, RoomUser user)
        {
            if (!this._gameStarted || user.Team == TeamType.NONE)
            {
                return;
            }

            if (roomitem.FreezePowerUp != FreezePowerUp.None)
            {
                this.PickUpPowerUp(roomitem, user);
            }
        }

        private void CountTeamPoints()
        {
            foreach (RoomUser user in this._roomInstance.GetTeamManager().GetAllPlayer())
            {
                this.CountTeamPoint(user);
            }
        }

        private void CountTeamPoint(RoomUser roomUser)
        {
            if (roomUser == null || roomUser.GetClient() == null)
            {
                return;
            }

            Item FirstTile = this.GetFirstTile(roomUser.X, roomUser.Y);

            if (FirstTile == null)
            {
                this.EndGame(roomUser, TeamType.NONE);
                return;
            }

            roomUser.BanzaiPowerUp = FreezePowerUp.None;
            roomUser.FreezeLives = 3;
            roomUser.ShieldActive = false;
            roomUser.ShieldCounter = 11;

            this._roomInstance.GetGameManager().AddPointToTeam(roomUser.Team, 40, null);

            roomUser.GetClient().SendPacket(new UpdateFreezeLivesComposer(roomUser.VirtualId, roomUser.FreezeLives));
        }

        private static void RemoveUserFromTeam(RoomUser user)
        {
            user.Team = TeamType.NONE;
            user.ApplyEffect(0);
        }

        public void throwBall(Item Item, RoomUser roomUserByUserId)
        {
            if (Math.Abs((roomUserByUserId.X - Item.X)) >= 2 || Math.Abs((roomUserByUserId.Y - Item.Y)) >= 2)
            {
                return;
            }

            if (roomUserByUserId.Team == TeamType.NONE || !this._gameStarted)
            {
                return;
            }

            Item Tile = this.GetFirstTile(Item.X, Item.Y);

            if (Tile == null)
            {
                return;
            }

            if (Tile.InteractionCountHelper == 0)
            {
                Tile.InteractionCountHelper = 1;
                Tile.ExtraData = "1000";
                Tile.UpdateState();
                Tile.InteractingUser = roomUserByUserId.UserId;
                Tile.FreezePowerUp = roomUserByUserId.BanzaiPowerUp;
                Tile.ReqUpdate(5);
                roomUserByUserId.CountFreezeBall -= 1;
                switch (roomUserByUserId.BanzaiPowerUp)
                {
                    case FreezePowerUp.GreenArrow:
                    case FreezePowerUp.OrangeSnowball:
                    case FreezePowerUp.BlueArrow:
                        roomUserByUserId.BanzaiPowerUp = FreezePowerUp.None;
                        break;
                }
            }
        }

        private Item GetFirstTile(int x, int y)
        {
            foreach (Item roomItem in this._roomInstance.GetGameMap().GetCoordinatedItems(new Point(x, y)))
            {
                if (roomItem.GetBaseItem().InteractionType == InteractionType.FREEZETILE)
                {
                    return roomItem;
                }
            }
            return null;
        }

        public void onFreezeTiles(Item item, FreezePowerUp powerUp, int userID)
        {
            if (this._roomInstance.GetRoomUserManager().GetRoomUserByUserId(userID) == null)
            {
                return;
            }

            List<Item> items;
            switch (powerUp)
            {
                case FreezePowerUp.BlueArrow:
                    items = this.GetVerticalItems(item.X, item.Y, 5);
                    break;
                case FreezePowerUp.GreenArrow:
                    items = this.GetDiagonalItems(item.X, item.Y, 5);
                    break;
                case FreezePowerUp.OrangeSnowball:
                    items = this.GetVerticalItems(item.X, item.Y, 5);
                    items.AddRange(this.GetDiagonalItems(item.X, item.Y, 5));
                    break;
                default:
                    items = this.GetVerticalItems(item.X, item.Y, 3);
                    break;
            }
            this.HandleBanzaiFreezeItems(items);
        }

        private static void ActivateShield(RoomUser user)
        {
            user.ApplyEffect(((int)user.Team + 48));
            user.ShieldActive = true;
            user.ShieldCounter = 0;
        }

        private void HandleBanzaiFreezeItems(List<Item> items)
        {
            foreach (Item roomItem in items)
            {
                switch (roomItem.GetBaseItem().InteractionType)
                {
                    case InteractionType.FREEZETILEBLOCK:
                        this.SetRandomPowerUp(roomItem);
                        roomItem.UpdateState(false, true);
                        continue;
                    case InteractionType.FREEZETILE:
                        roomItem.ExtraData = "11000";
                        roomItem.UpdateState(false, true);
                        continue;
                    default:
                        continue;
                }
            }
        }

        private void SetRandomPowerUp(Item item)
        {
            if (!string.IsNullOrEmpty(item.ExtraData))
            {
                return;
            }

            switch (ButterflyEnvironment.GetRandomNumber(1, 14))
            {
                case 2:
                    item.ExtraData = "2000";
                    item.FreezePowerUp = FreezePowerUp.BlueArrow;
                    break;
                case 3:
                    item.ExtraData = "3000";
                    item.FreezePowerUp = FreezePowerUp.Snowballs;
                    break;
                case 4:
                    item.ExtraData = "4000";
                    item.FreezePowerUp = FreezePowerUp.GreenArrow;
                    break;
                case 5:
                    item.ExtraData = "5000";
                    item.FreezePowerUp = FreezePowerUp.OrangeSnowball;
                    break;
                case 6:
                    item.ExtraData = "6000";
                    item.FreezePowerUp = FreezePowerUp.Heart;
                    break;
                case 7:
                    item.ExtraData = "7000";
                    item.FreezePowerUp = FreezePowerUp.Shield;
                    break;
                default:
                    item.ExtraData = "1000";
                    item.FreezePowerUp = FreezePowerUp.None;
                    break;
            }
            this._roomInstance.GetGameMap().UpdateMapForItem(item);
            item.UpdateState(false, true);
        }

        private void PickUpPowerUp(Item item, RoomUser user)
        {
            switch (item.FreezePowerUp)
            {
                case FreezePowerUp.BlueArrow:
                case FreezePowerUp.GreenArrow:
                case FreezePowerUp.OrangeSnowball:
                    user.BanzaiPowerUp = item.FreezePowerUp;
                    break;
                case FreezePowerUp.Shield:
                    ActivateShield(user);
                    break;
                case FreezePowerUp.Heart:
                    if (user.FreezeLives < 3)
                    {
                        user.FreezeLives++;
                        this._roomInstance.GetGameManager().AddPointToTeam(user.Team, 10, user);
                    }

                    user.GetClient().SendPacket(new UpdateFreezeLivesComposer(user.VirtualId, user.FreezeLives));
                    break;
                case FreezePowerUp.Snowballs:
                    user.CountFreezeBall += 1;
                    break;
            }
            item.FreezePowerUp = FreezePowerUp.None;
            item.ExtraData = "1" + item.ExtraData;
            item.UpdateState(false, true);
        }

        public void AddFreezeBlock(Item item)
        {
            if (this._freezeBlocks.ContainsKey(item.Id))
            {
                this._freezeBlocks.Remove(item.Id);
            }

            this._freezeBlocks.Add(item.Id, item);
        }

        public void RemoveFreezeBlock(int itemID)
        {
            this._freezeBlocks.Remove(itemID);
        }

        private void HandleUserFreeze(Point point)
        {
            RoomUser user = this._roomInstance.GetRoomUserManager().GetUserForSquare(point.X, point.Y);
            if (user == null || user.IsWalking && user.SetX != point.X && user.SetY != point.Y)
            {
                return;
            }

            this.FreezeUser(user);
        }

        private void FreezeUser(RoomUser user)
        {
            if (user.IsBot || user.ShieldActive || user.Team == TeamType.NONE || !this._gameStarted)
            {
                return;
            }

            if (user.Freezed)
            {
                user.Freezed = false;
                user.ApplyEffect(((int)user.Team + 39));
            }
            else
            {
                user.Freezed = true;
                user.FreezeCounter = 0;
                --user.FreezeLives;
                if (user.FreezeLives <= 0)
                {
                    user.GetClient().SendPacket(new UpdateFreezeLivesComposer(user.VirtualId, user.FreezeLives));

                    user.ApplyEffect(0);

                    this._roomInstance.GetGameManager().AddPointToTeam(user.Team, -20, user);

                    TeamManager managerForFreeze = this._roomInstance.GetTeamManager();
                    managerForFreeze.OnUserLeave(user);

                    user.GetClient().SendPacket(new IsPlayingComposer(false));

                    this._roomInstance.GetGameManager().UpdateGatesTeamCounts();
                    user.Team = TeamType.NONE;

                    if (this._roomInstance.GetGameItemHandler().GetExitTeleport() != null)
                    {
                        this._roomInstance.GetGameMap().TeleportToItem(user, this._roomInstance.GetGameItemHandler().GetExitTeleport());
                    }

                    user.Freezed = false;
                    user.SetStep = false;
                    user.IsWalking = false;
                    user.UpdateNeeded = true;

                    if (managerForFreeze.BlueTeam.Count <= 0 && managerForFreeze.RedTeam.Count <= 0 && (managerForFreeze.GreenTeam.Count <= 0 && managerForFreeze.YellowTeam.Count > 0))
                    {
                        this.StopGame();
                    }
                    else if (managerForFreeze.BlueTeam.Count > 0 && managerForFreeze.RedTeam.Count <= 0 && (managerForFreeze.GreenTeam.Count <= 0 && managerForFreeze.YellowTeam.Count <= 0))
                    {
                        this.StopGame();
                    }
                    else if (managerForFreeze.BlueTeam.Count <= 0 && managerForFreeze.RedTeam.Count > 0 && (managerForFreeze.GreenTeam.Count <= 0 && managerForFreeze.YellowTeam.Count <= 0))
                    {
                        this.StopGame();
                    }
                    else
                    {
                        if (managerForFreeze.BlueTeam.Count > 0 || managerForFreeze.RedTeam.Count > 0 || (managerForFreeze.GreenTeam.Count <= 0 || managerForFreeze.YellowTeam.Count > 0))
                        {
                            return;
                        }

                        this.StopGame();
                    }
                }
                else
                {
                    this._roomInstance.GetGameManager().AddPointToTeam(user.Team, -10, user);
                    user.ApplyEffect(12);

                    user.GetClient().SendPacket(new UpdateFreezeLivesComposer(user.VirtualId, user.FreezeLives));
                }
            }
        }

        private List<Item> GetVerticalItems(int x, int y, int length)
        {
            List<Item> list = new List<Item>();
            for (int index = 0; index < length; ++index)
            {
                Point point = new Point(x + index, y);
                List<Item> itemsForSquare = this.GetItemsForSquare(point);
                if (SquareGotFreezeTile(itemsForSquare))
                {
                    this.HandleUserFreeze(point);
                    list.AddRange(itemsForSquare);
                    if (SquareGotFreezeBlock(itemsForSquare))
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            for (int index = 1; index < length; ++index)
            {
                Point point = new Point(x, y + index);
                List<Item> itemsForSquare = this.GetItemsForSquare(point);
                if (SquareGotFreezeTile(itemsForSquare))
                {
                    this.HandleUserFreeze(point);
                    list.AddRange(itemsForSquare);
                    if (SquareGotFreezeBlock(itemsForSquare))
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            for (int index = 1; index < length; ++index)
            {
                Point point = new Point(x - index, y);
                List<Item> itemsForSquare = this.GetItemsForSquare(point);
                if (SquareGotFreezeTile(itemsForSquare))
                {
                    this.HandleUserFreeze(point);
                    list.AddRange(itemsForSquare);
                    if (SquareGotFreezeBlock(itemsForSquare))
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            for (int index = 1; index < length; ++index)
            {
                Point point = new Point(x, y - index);
                List<Item> itemsForSquare = this.GetItemsForSquare(point);
                if (SquareGotFreezeTile(itemsForSquare))
                {
                    this.HandleUserFreeze(point);
                    list.AddRange(itemsForSquare);
                    if (SquareGotFreezeBlock(itemsForSquare))
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            return list;
        }

        private List<Item> GetDiagonalItems(int x, int y, int length)
        {
            List<Item> list = new List<Item>();
            for (int index = 0; index < length; ++index)
            {
                Point point = new Point(x + index, y + index);
                List<Item> itemsForSquare = this.GetItemsForSquare(point);
                if (SquareGotFreezeTile(itemsForSquare))
                {
                    this.HandleUserFreeze(point);
                    list.AddRange(itemsForSquare);
                    if (SquareGotFreezeBlock(itemsForSquare))
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            for (int index = 0; index < length; ++index)
            {
                Point point = new Point(x - index, y - index);
                List<Item> itemsForSquare = this.GetItemsForSquare(point);
                if (SquareGotFreezeTile(itemsForSquare))
                {
                    this.HandleUserFreeze(point);
                    list.AddRange(itemsForSquare);
                    if (SquareGotFreezeBlock(itemsForSquare))
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            for (int index = 0; index < length; ++index)
            {
                Point point = new Point(x - index, y + index);
                List<Item> itemsForSquare = this.GetItemsForSquare(point);
                if (SquareGotFreezeTile(itemsForSquare))
                {
                    this.HandleUserFreeze(point);
                    list.AddRange(itemsForSquare);
                    if (SquareGotFreezeBlock(itemsForSquare))
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            for (int index = 0; index < length; ++index)
            {
                Point point = new Point(x + index, y - index);
                List<Item> itemsForSquare = this.GetItemsForSquare(point);
                if (SquareGotFreezeTile(itemsForSquare))
                {
                    this.HandleUserFreeze(point);
                    list.AddRange(itemsForSquare);
                    if (SquareGotFreezeBlock(itemsForSquare))
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            return list;
        }

        private List<Item> GetItemsForSquare(Point point)
        {
            return this._roomInstance.GetGameMap().GetCoordinatedItems(point);
        }

        private static bool SquareGotFreezeTile(List<Item> items)
        {
            foreach (Item roomItem in items)
            {
                if (roomItem.GetBaseItem().InteractionType == InteractionType.FREEZETILE)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool SquareGotFreezeBlock(List<Item> items)
        {
            foreach (Item roomItem in items)
            {
                if (roomItem.GetBaseItem().InteractionType == InteractionType.FREEZETILEBLOCK)
                {
                    return true;
                }
            }
            return false;
        }

        public void Destroy()
        {
            this._freezeBlocks.Clear();
            this._roomInstance = null;
        }
    }
}
