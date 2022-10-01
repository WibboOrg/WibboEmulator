using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Map.Movement;
using WibboEmulator.Games.Items.Wired.Interfaces;
using System.Data;
using System.Drawing;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;

namespace WibboEmulator.Games.Items.Wired.Actions
{
    public class MoveToDir : WiredActionBase, IWiredEffect, IWired
    {
        private MovementDirection MoveToDirMovement;

        public MoveToDir(Item item, Room room) : base(item, room, (int)WiredActionType.MOVE_TO_DIRECTION)
        {
            this.MoveToDirMovement = MovementDirection.none;

            this.IntParams.Add(0);
            this.IntParams.Add(0);
        }

        public override bool OnCycle(RoomUser user, Item item)
        {
            foreach (Item roomItem in this.Items)
            {
                this.HandleMovement(roomItem);
            }

            return false;
        }
        
        public override void LoadItems(bool inDatabase = false)
        {
            base.LoadItems();

            MovementDirection startDirection = (MovementDirection)((this.IntParams.Count > 0) ? this.IntParams[0] : 0);

            this.MoveToDirMovement = startDirection;
        }

        private void HandleMovement(Item item)
        {
            if (this.RoomInstance.GetRoomItemHandler().GetItem(item.Id) == null)
            {
                return;
            }

            Point newPoint = MovementUtility.HandleMovementDir(item.X, item.Y, this.MoveToDirMovement);

            RoomUser roomUser = this.RoomInstance.GetRoomUserManager().GetUserForSquare(newPoint.X, newPoint.Y);
            if (roomUser != null)
            {
                this.RoomInstance.GetWiredHandler().TriggerCollision(roomUser, item);
                return;
            }

            MovementDirection startDirection = (MovementDirection)((this.IntParams.Count > 0) ? this.IntParams[0] : 0);
            WhenMovementBlock whenMoveIsBlocked = (WhenMovementBlock)((this.IntParams.Count > 1) ? this.IntParams[1] : 0);

            int oldX = item.X;
            int oldY = item.Y;
            double oldZ = item.Z;
            if (this.RoomInstance.GetRoomItemHandler().SetFloorItem(null, item, newPoint.X, newPoint.Y, item.Rotation, false, false, false))
            {
                this.RoomInstance.SendPacket(new SlideObjectBundleComposer(oldX, oldY, oldZ, newPoint.X, newPoint.Y, item.Z, item.Id));
                return;
            }

            switch (whenMoveIsBlocked)
            {
                case WhenMovementBlock.none:
                    {
                        //this.movetodirMovement = MovementDirection.none;
                        break;
                    }
                case WhenMovementBlock.right45:
                    {
                        if (this.MoveToDirMovement == MovementDirection.right)
                        {
                            if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                            {
                                this.MoveToDirMovement = MovementDirection.downright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y + 1)) // abajo
                            {
                                this.MoveToDirMovement = MovementDirection.down;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                            {
                                this.MoveToDirMovement = MovementDirection.downleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y)) // izq
                            {
                                this.MoveToDirMovement = MovementDirection.left;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                            {
                                this.MoveToDirMovement = MovementDirection.upleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y - 1)) // arriba
                            {
                                this.MoveToDirMovement = MovementDirection.up;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                            {
                                this.MoveToDirMovement = MovementDirection.upright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y)) // derecha
                            {
                                this.MoveToDirMovement = MovementDirection.right;
                                break;
                            }
                        }
                        else if (this.MoveToDirMovement == MovementDirection.left)
                        {
                            if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                            {
                                this.MoveToDirMovement = MovementDirection.upleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y - 1)) // arriba
                            {
                                this.MoveToDirMovement = MovementDirection.up;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                            {
                                this.MoveToDirMovement = MovementDirection.upright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y)) // derecha
                            {
                                this.MoveToDirMovement = MovementDirection.right;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                            {
                                this.MoveToDirMovement = MovementDirection.downright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y + 1)) // abajo
                            {
                                this.MoveToDirMovement = MovementDirection.down;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                            {
                                this.MoveToDirMovement = MovementDirection.downleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y)) // izq
                            {
                                this.MoveToDirMovement = MovementDirection.left;
                                break;
                            }
                        }
                        else if (this.MoveToDirMovement == MovementDirection.up)
                        {
                            if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                            {
                                this.MoveToDirMovement = MovementDirection.upright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y)) // derecha
                            {
                                this.MoveToDirMovement = MovementDirection.right;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                            {
                                this.MoveToDirMovement = MovementDirection.downright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y + 1)) // abajo
                            {
                                this.MoveToDirMovement = MovementDirection.down;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                            {
                                this.MoveToDirMovement = MovementDirection.downleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y)) // izq
                            {
                                this.MoveToDirMovement = MovementDirection.left;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                            {
                                this.MoveToDirMovement = MovementDirection.upleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y - 1)) // arriba
                            {
                                this.MoveToDirMovement = MovementDirection.up;
                                break;
                            }
                        }
                        else if (this.MoveToDirMovement == MovementDirection.down)
                        {
                            if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                            {
                                this.MoveToDirMovement = MovementDirection.downleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y)) // izq
                            {
                                this.MoveToDirMovement = MovementDirection.left;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                            {
                                this.MoveToDirMovement = MovementDirection.upleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y - 1)) // arriba
                            {
                                this.MoveToDirMovement = MovementDirection.up;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                            {
                                this.MoveToDirMovement = MovementDirection.upright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y)) // derecha
                            {
                                this.MoveToDirMovement = MovementDirection.right;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                            {
                                this.MoveToDirMovement = MovementDirection.downright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y + 1)) // abajo
                            {
                                this.MoveToDirMovement = MovementDirection.down;
                                break;
                            }
                        }
                        else if (this.MoveToDirMovement == MovementDirection.upleft)
                        {
                            if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y - 1)) // arriba
                            {
                                this.MoveToDirMovement = MovementDirection.up;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                            {
                                this.MoveToDirMovement = MovementDirection.upright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y)) // derecha
                            {
                                this.MoveToDirMovement = MovementDirection.right;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                            {
                                this.MoveToDirMovement = MovementDirection.downright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y + 1)) // abajo
                            {
                                this.MoveToDirMovement = MovementDirection.down;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                            {
                                this.MoveToDirMovement = MovementDirection.downleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y)) // izq
                            {
                                this.MoveToDirMovement = MovementDirection.left;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                            {
                                this.MoveToDirMovement = MovementDirection.upleft;
                                break;
                            }
                        }
                        else if (this.MoveToDirMovement == MovementDirection.upright)
                        {
                            if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y)) // derecha
                            {
                                this.MoveToDirMovement = MovementDirection.right;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                            {
                                this.MoveToDirMovement = MovementDirection.downright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y + 1)) // abajo
                            {
                                this.MoveToDirMovement = MovementDirection.down;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                            {
                                this.MoveToDirMovement = MovementDirection.downleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y)) // izq
                            {
                                this.MoveToDirMovement = MovementDirection.left;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                            {
                                this.MoveToDirMovement = MovementDirection.upleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y - 1)) // arriba
                            {
                                this.MoveToDirMovement = MovementDirection.up;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                            {
                                this.MoveToDirMovement = MovementDirection.upright;
                                break;
                            }
                            return;
                        }
                        else if (this.MoveToDirMovement == MovementDirection.downright)
                        {
                            if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y + 1)) // abajo
                            {
                                this.MoveToDirMovement = MovementDirection.down;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                            {
                                this.MoveToDirMovement = MovementDirection.downleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y)) // izq
                            {
                                this.MoveToDirMovement = MovementDirection.left;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                            {
                                this.MoveToDirMovement = MovementDirection.upleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y - 1)) // arriba
                            {
                                this.MoveToDirMovement = MovementDirection.up;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                            {
                                this.MoveToDirMovement = MovementDirection.upright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y)) // derecha
                            {
                                this.MoveToDirMovement = MovementDirection.right;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                            {
                                this.MoveToDirMovement = MovementDirection.downright;
                                break;
                            }
                        }
                        else if (this.MoveToDirMovement == MovementDirection.downleft)
                        {
                            if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y)) // izq
                            {
                                this.MoveToDirMovement = MovementDirection.left;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                            {
                                this.MoveToDirMovement = MovementDirection.upleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y - 1)) // arriba
                            {
                                this.MoveToDirMovement = MovementDirection.up;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                            {
                                this.MoveToDirMovement = MovementDirection.upright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y)) // derecha
                            {
                                this.MoveToDirMovement = MovementDirection.right;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                            {
                                this.MoveToDirMovement = MovementDirection.downright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y + 1)) // abajo
                            {
                                this.MoveToDirMovement = MovementDirection.down;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                            {
                                this.MoveToDirMovement = MovementDirection.downleft;
                                break;
                            }
                        }

                        break;
                    }
                case WhenMovementBlock.right90:
                    {
                        if (this.MoveToDirMovement == MovementDirection.right)
                        {
                            if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y + 1)) // abajo
                            {
                                this.MoveToDirMovement = MovementDirection.down;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y)) // izq
                            {
                                this.MoveToDirMovement = MovementDirection.left;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y - 1)) // arriba
                            {
                                this.MoveToDirMovement = MovementDirection.up;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y)) // derecha
                            {
                                this.MoveToDirMovement = MovementDirection.right;
                                break;
                            }
                        }
                        else if (this.MoveToDirMovement == MovementDirection.left)
                        {
                            if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y - 1)) // arriba
                            {
                                this.MoveToDirMovement = MovementDirection.up;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y)) // derecha
                            {
                                this.MoveToDirMovement = MovementDirection.right;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y + 1)) // abajo
                            {
                                this.MoveToDirMovement = MovementDirection.down;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y)) // izq
                            {
                                this.MoveToDirMovement = MovementDirection.left;
                                break;
                            }
                        }
                        else if (this.MoveToDirMovement == MovementDirection.up)
                        {
                            if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y)) // derecha
                            {
                                this.MoveToDirMovement = MovementDirection.right;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y + 1)) // abajo
                            {
                                this.MoveToDirMovement = MovementDirection.down;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y)) // izq
                            {
                                this.MoveToDirMovement = MovementDirection.left;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y - 1)) // arriba
                            {
                                this.MoveToDirMovement = MovementDirection.up;
                                break;
                            }
                        }
                        else if (this.MoveToDirMovement == MovementDirection.down)
                        {
                            if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y)) // izq
                            {
                                this.MoveToDirMovement = MovementDirection.left;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y - 1)) // arriba
                            {
                                this.MoveToDirMovement = MovementDirection.up;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y)) // derecha
                            {
                                this.MoveToDirMovement = MovementDirection.right;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y + 1)) // abajo
                            {
                                this.MoveToDirMovement = MovementDirection.down;
                                break;
                            }
                        }
                        else if (this.MoveToDirMovement == MovementDirection.upleft)
                        {
                            if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                            {
                                this.MoveToDirMovement = MovementDirection.upright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                            {
                                this.MoveToDirMovement = MovementDirection.downright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                            {
                                this.MoveToDirMovement = MovementDirection.downleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                            {
                                this.MoveToDirMovement = MovementDirection.upleft;
                                break;
                            }
                        }
                        else if (this.MoveToDirMovement == MovementDirection.upright)
                        {
                            if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                            {
                                this.MoveToDirMovement = MovementDirection.downright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                            {
                                this.MoveToDirMovement = MovementDirection.downleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                            {
                                this.MoveToDirMovement = MovementDirection.upleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                            {
                                this.MoveToDirMovement = MovementDirection.upright;
                                break;
                            }
                            return;
                        }
                        else if (this.MoveToDirMovement == MovementDirection.downright)
                        {
                            if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                            {
                                this.MoveToDirMovement = MovementDirection.downleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                            {
                                this.MoveToDirMovement = MovementDirection.upleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                            {
                                this.MoveToDirMovement = MovementDirection.upright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                            {
                                this.MoveToDirMovement = MovementDirection.downright;
                                break;
                            }
                        }
                        else if (this.MoveToDirMovement == MovementDirection.downleft)
                        {
                            if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                            {
                                this.MoveToDirMovement = MovementDirection.upleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                            {
                                this.MoveToDirMovement = MovementDirection.upright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                            {
                                this.MoveToDirMovement = MovementDirection.downright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                            {
                                this.MoveToDirMovement = MovementDirection.downleft;
                                break;
                            }
                        }

                        break;
                    }
                case WhenMovementBlock.left45:
                    {
                        if (this.MoveToDirMovement == MovementDirection.right)
                        {
                            if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                            {
                                this.MoveToDirMovement = MovementDirection.upright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y - 1)) // arriba
                            {
                                this.MoveToDirMovement = MovementDirection.up;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                            {
                                this.MoveToDirMovement = MovementDirection.upleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y)) // izq
                            {
                                this.MoveToDirMovement = MovementDirection.left;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                            {
                                this.MoveToDirMovement = MovementDirection.downleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y + 1)) // abajo
                            {
                                this.MoveToDirMovement = MovementDirection.down;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                            {
                                this.MoveToDirMovement = MovementDirection.downright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y)) // derecha
                            {
                                this.MoveToDirMovement = MovementDirection.right;
                                break;
                            }
                        }
                        else if (this.MoveToDirMovement == MovementDirection.left)
                        {
                            if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                            {
                                this.MoveToDirMovement = MovementDirection.downleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y + 1)) // abajo
                            {
                                this.MoveToDirMovement = MovementDirection.down;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                            {
                                this.MoveToDirMovement = MovementDirection.downright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y)) // derecha
                            {
                                this.MoveToDirMovement = MovementDirection.right;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                            {
                                this.MoveToDirMovement = MovementDirection.upright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y - 1)) // arriba
                            {
                                this.MoveToDirMovement = MovementDirection.up;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                            {
                                this.MoveToDirMovement = MovementDirection.upleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y)) // izq
                            {
                                this.MoveToDirMovement = MovementDirection.left;
                                break;
                            }
                        }
                        else if (this.MoveToDirMovement == MovementDirection.up)
                        {
                            if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                            {
                                this.MoveToDirMovement = MovementDirection.upleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y)) // izq
                            {
                                this.MoveToDirMovement = MovementDirection.left;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                            {
                                this.MoveToDirMovement = MovementDirection.downleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y + 1)) // abajo
                            {
                                this.MoveToDirMovement = MovementDirection.down;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                            {
                                this.MoveToDirMovement = MovementDirection.downright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y)) // derecha
                            {
                                this.MoveToDirMovement = MovementDirection.right;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                            {
                                this.MoveToDirMovement = MovementDirection.upright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y - 1)) // arriba
                            {
                                this.MoveToDirMovement = MovementDirection.up;
                                break;
                            }
                        }
                        else if (this.MoveToDirMovement == MovementDirection.down)
                        {
                            if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                            {
                                this.MoveToDirMovement = MovementDirection.downright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y)) // derecha
                            {
                                this.MoveToDirMovement = MovementDirection.right;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                            {
                                this.MoveToDirMovement = MovementDirection.upright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y - 1)) // arriba
                            {
                                this.MoveToDirMovement = MovementDirection.up;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                            {
                                this.MoveToDirMovement = MovementDirection.upleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y)) // izq
                            {
                                this.MoveToDirMovement = MovementDirection.left;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                            {
                                this.MoveToDirMovement = MovementDirection.downleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y + 1)) // abajo
                            {
                                this.MoveToDirMovement = MovementDirection.down;
                                break;
                            }
                        }
                        else if (this.MoveToDirMovement == MovementDirection.upleft)
                        {
                            if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y)) // izq
                            {
                                this.MoveToDirMovement = MovementDirection.left;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                            {
                                this.MoveToDirMovement = MovementDirection.downleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y + 1)) // abajo
                            {
                                this.MoveToDirMovement = MovementDirection.down;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                            {
                                this.MoveToDirMovement = MovementDirection.downright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y)) // derecha
                            {
                                this.MoveToDirMovement = MovementDirection.right;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                            {
                                this.MoveToDirMovement = MovementDirection.upright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y - 1)) // arriba
                            {
                                this.MoveToDirMovement = MovementDirection.up;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                            {
                                this.MoveToDirMovement = MovementDirection.upleft;
                                break;
                            }
                        }
                        else if (this.MoveToDirMovement == MovementDirection.upright)
                        {
                            if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y - 1)) // arriba
                            {
                                this.MoveToDirMovement = MovementDirection.up;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                            {
                                this.MoveToDirMovement = MovementDirection.upleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y)) // izq
                            {
                                this.MoveToDirMovement = MovementDirection.left;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                            {
                                this.MoveToDirMovement = MovementDirection.downleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y + 1)) // abajo
                            {
                                this.MoveToDirMovement = MovementDirection.down;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                            {
                                this.MoveToDirMovement = MovementDirection.downright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y)) // derecha
                            {
                                this.MoveToDirMovement = MovementDirection.right;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                            {
                                this.MoveToDirMovement = MovementDirection.upright;
                                break;
                            }
                        }
                        else if (this.MoveToDirMovement == MovementDirection.downright)
                        {
                            if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y)) // derecha
                            {
                                this.MoveToDirMovement = MovementDirection.right;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                            {
                                this.MoveToDirMovement = MovementDirection.upright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y - 1)) // arriba
                            {
                                this.MoveToDirMovement = MovementDirection.up;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                            {
                                this.MoveToDirMovement = MovementDirection.upleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y)) // izq
                            {
                                this.MoveToDirMovement = MovementDirection.left;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                            {
                                this.MoveToDirMovement = MovementDirection.downleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y + 1)) // abajo
                            {
                                this.MoveToDirMovement = MovementDirection.down;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                            {
                                this.MoveToDirMovement = MovementDirection.downright;
                                break;
                            }
                        }
                        else if (this.MoveToDirMovement == MovementDirection.downleft)
                        {
                            if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y + 1)) // abajo
                            {
                                this.MoveToDirMovement = MovementDirection.down;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                            {
                                this.MoveToDirMovement = MovementDirection.downright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y)) // derecha
                            {
                                this.MoveToDirMovement = MovementDirection.right;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                            {
                                this.MoveToDirMovement = MovementDirection.upright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y - 1)) // arriba
                            {
                                this.MoveToDirMovement = MovementDirection.up;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                            {
                                this.MoveToDirMovement = MovementDirection.upleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y)) // izq
                            {
                                this.MoveToDirMovement = MovementDirection.left;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                            {
                                this.MoveToDirMovement = MovementDirection.downleft;
                                break;
                            }
                        }

                        break;
                    }
                case WhenMovementBlock.left90:
                    {
                        if (this.MoveToDirMovement == MovementDirection.right)
                        {
                            if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y - 1)) // arriba
                            {
                                this.MoveToDirMovement = MovementDirection.up;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y)) // izq
                            {
                                this.MoveToDirMovement = MovementDirection.left;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y + 1)) // abajo
                            {
                                this.MoveToDirMovement = MovementDirection.down;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y)) // derecha
                            {
                                this.MoveToDirMovement = MovementDirection.right;
                                break;
                            }
                        }
                        else if (this.MoveToDirMovement == MovementDirection.left)
                        {
                            if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y + 1)) // abajo
                            {
                                this.MoveToDirMovement = MovementDirection.down;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y)) // derecha
                            {
                                this.MoveToDirMovement = MovementDirection.right;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y - 1)) // arriba
                            {
                                this.MoveToDirMovement = MovementDirection.up;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y)) // izq
                            {
                                this.MoveToDirMovement = MovementDirection.left;
                                break;
                            }
                        }
                        else if (this.MoveToDirMovement == MovementDirection.up)
                        {
                            if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y)) // izq
                            {
                                this.MoveToDirMovement = MovementDirection.left;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y + 1)) // abajo
                            {
                                this.MoveToDirMovement = MovementDirection.down;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y)) // derecha
                            {
                                this.MoveToDirMovement = MovementDirection.right;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y - 1)) // arriba
                            {
                                this.MoveToDirMovement = MovementDirection.up;
                                break;
                            }
                        }
                        else if (this.MoveToDirMovement == MovementDirection.down)
                        {
                            if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y)) // derecha
                            {
                                this.MoveToDirMovement = MovementDirection.right;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y - 1)) // arriba
                            {
                                this.MoveToDirMovement = MovementDirection.up;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y)) // izq
                            {
                                this.MoveToDirMovement = MovementDirection.left;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X, item.Y + 1)) // abajo
                            {
                                this.MoveToDirMovement = MovementDirection.down;
                                break;
                            }
                        }
                        else if (this.MoveToDirMovement == MovementDirection.upleft)
                        {
                            if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                            {
                                this.MoveToDirMovement = MovementDirection.downleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                            {
                                this.MoveToDirMovement = MovementDirection.downright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                            {
                                this.MoveToDirMovement = MovementDirection.upright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                            {
                                this.MoveToDirMovement = MovementDirection.upleft;
                                break;
                            }
                        }
                        else if (this.MoveToDirMovement == MovementDirection.upright)
                        {
                            if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                            {
                                this.MoveToDirMovement = MovementDirection.upleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                            {
                                this.MoveToDirMovement = MovementDirection.downleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                            {
                                this.MoveToDirMovement = MovementDirection.downright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                            {
                                this.MoveToDirMovement = MovementDirection.upright;
                                break;
                            }
                        }
                        else if (this.MoveToDirMovement == MovementDirection.downright)
                        {
                            if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                            {
                                this.MoveToDirMovement = MovementDirection.upright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                            {
                                this.MoveToDirMovement = MovementDirection.upleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                            {
                                this.MoveToDirMovement = MovementDirection.downleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                            {
                                this.MoveToDirMovement = MovementDirection.downright;
                                break;
                            }
                        }
                        else if (this.MoveToDirMovement == MovementDirection.downleft)
                        {
                            if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                            {
                                this.MoveToDirMovement = MovementDirection.downright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                            {
                                this.MoveToDirMovement = MovementDirection.upright;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                            {
                                this.MoveToDirMovement = MovementDirection.upleft;
                                break;
                            }
                            else if (this.RoomInstance.GetGameMap().CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                            {
                                this.MoveToDirMovement = MovementDirection.downleft;
                                break;
                            }
                        }

                        break;
                    }
                case WhenMovementBlock.turnback:
                    {
                        if (this.MoveToDirMovement == MovementDirection.right)
                        {
                            this.MoveToDirMovement = MovementDirection.left;
                        }
                        else if (this.MoveToDirMovement == MovementDirection.left)
                        {
                            this.MoveToDirMovement = MovementDirection.right;
                        }
                        else if (this.MoveToDirMovement == MovementDirection.up)
                        {
                            this.MoveToDirMovement = MovementDirection.down;
                        }
                        else if (this.MoveToDirMovement == MovementDirection.down)
                        {
                            this.MoveToDirMovement = MovementDirection.up;
                        }
                        else if (this.MoveToDirMovement == MovementDirection.upright)
                        {
                            this.MoveToDirMovement = MovementDirection.downleft;
                        }
                        else if (this.MoveToDirMovement == MovementDirection.downleft)
                        {
                            this.MoveToDirMovement = MovementDirection.upright;
                        }
                        else if (this.MoveToDirMovement == MovementDirection.upleft)
                        {
                            this.MoveToDirMovement = MovementDirection.downright;
                        }
                        else if (this.MoveToDirMovement == MovementDirection.downright)
                        {
                            this.MoveToDirMovement = MovementDirection.upleft;
                        }
                        break;
                    }
                case WhenMovementBlock.turnrandom:
                    {
                        this.MoveToDirMovement = (MovementDirection)WibboEnvironment.GetRandomNumber(1, 7);
                        break;
                    }
            }

            newPoint = MovementUtility.HandleMovementDir(item.X, item.Y, this.MoveToDirMovement);

            if (newPoint != item.Coordinate)
            {
                oldX = item.X;
                oldY = item.Y;
                oldZ = item.Z;

                if (this.RoomInstance.GetRoomItemHandler().SetFloorItem(null, item, newPoint.X, newPoint.Y, item.Rotation, false, false, false))
                {
                    this.RoomInstance.SendPacket(new SlideObjectBundleComposer(oldX, oldY, oldZ, newPoint.X, newPoint.Y, item.Z, item.Id));
                }
            }
            return;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            MovementDirection startDirection = (MovementDirection)((this.IntParams.Count > 0) ? this.IntParams[0] : 0);
            WhenMovementBlock whenMoveIsBlocked = (WhenMovementBlock)((this.IntParams.Count > 1) ? this.IntParams[1] : 0);

            WiredUtillity.SaveTriggerItem(dbClient, this.Id, Convert.ToInt32(startDirection).ToString(), Convert.ToInt32(whenMoveIsBlocked).ToString(), false, this.Items, this.Delay);
        }

        public void LoadFromDatabase(DataRow row)
        {
            this.IntParams.Clear();

            if (int.TryParse(row["delay"].ToString(), out int delay))
	            this.Delay = delay;
                
            string triggerItems = row["triggers_item"].ToString();

            if (int.TryParse(row["trigger_data_2"].ToString(), out int startDirection))
                this.IntParams.Add(startDirection);

            if (int.TryParse(row["trigger_data"].ToString(), out int whenMoveIsBlocked))
                this.IntParams.Add(whenMoveIsBlocked);

            if (triggerItems == "")
            {
                return;
            }

            foreach (string itemId in triggerItems.Split(';'))
            {
                if (!int.TryParse(itemId, out int id))
                    continue;

                if(!this.StuffIds.Contains(id))
                    this.StuffIds.Add(id);
            }
        }
    }
}
