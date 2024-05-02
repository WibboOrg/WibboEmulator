namespace WibboEmulator.Games.Items.Wired.Actions;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Map.Movement;

public class MoveToDir : WiredActionBase, IWiredEffect, IWired
{
    private MovementDirection _moveToDirMovement;

    public MoveToDir(Item item, Room room) : base(item, room, (int)WiredActionType.MOVE_TO_DIRECTION)
    {
        this._moveToDirMovement = MovementDirection.none;

        this.DefaultIntParams(new int[] { 0, 0 });
    }

    public override bool OnCycle(RoomUser user, Item item)
    {
        var disableAnimation = this.Room.WiredHandler.DisableAnimate(this.Item.Coordinate);

        foreach (var roomItem in this.Items.ToList())
        {
            this.HandleMovement(roomItem, disableAnimation);
        }

        return false;
    }

    public override void LoadItems(bool inDatabase = false)
    {
        base.LoadItems();

        var startDirection = (MovementDirection)this.GetIntParam(0);

        this._moveToDirMovement = startDirection;
    }

    private void HandleMovement(Item roomItem, bool disableAnimation)
    {
        if (this.Room.RoomItemHandling.GetItem(roomItem.Id) == null)
        {
            return;
        }

        var newPoint = MovementUtility.HandleMovementDir(roomItem.X, roomItem.Y, this._moveToDirMovement);

        var roomUser = this.Room.RoomUserManager.GetUserForSquare(newPoint.X, newPoint.Y);
        if (roomUser != null)
        {
            this.Room.WiredHandler.TriggerCollision(roomUser, roomItem);
            return;
        }

        //var startDirection = (MovementDirection)(this.GetIntParam(0));
        var whenMoveIsBlocked = (WhenMovementBlock)this.GetIntParam(1);

        var oldX = disableAnimation ? newPoint.X : roomItem.X;
        var oldY = disableAnimation ? newPoint.Y : roomItem.Y;
        var oldZ = roomItem.Z;
        if (this.Room.RoomItemHandling.SetFloorItem(null, roomItem, newPoint.X, newPoint.Y, roomItem.Rotation, false, false, false))
        {
            this.Room.SendPacket(new SlideObjectBundleComposer(oldX, oldY, disableAnimation ? roomItem.Z : oldZ, newPoint.X, newPoint.Y, roomItem.Z, roomItem.Id));
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
                if (this._moveToDirMovement == MovementDirection.right)
                {
                    if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.left)
                {
                    if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.up)
                {
                    if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.down)
                {
                    if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.upleft)
                {
                    if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.upright)
                {
                    if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    return;
                }
                else if (this._moveToDirMovement == MovementDirection.downright)
                {
                    if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.downleft)
                {
                    if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                }

                break;
            }
            case WhenMovementBlock.right90:
            {
                if (this._moveToDirMovement == MovementDirection.right)
                {
                    if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.left)
                {
                    if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.up)
                {
                    if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.down)
                {
                    if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.upleft)
                {
                    if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.upright)
                {
                    if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    return;
                }
                else if (this._moveToDirMovement == MovementDirection.downright)
                {
                    if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.downleft)
                {
                    if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                }

                break;
            }
            case WhenMovementBlock.left45:
            {
                if (this._moveToDirMovement == MovementDirection.right)
                {
                    if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.left)
                {
                    if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.up)
                {
                    if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.down)
                {
                    if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.upleft)
                {
                    if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.upright)
                {
                    if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.downright)
                {
                    if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.downleft)
                {
                    if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                }

                break;
            }
            case WhenMovementBlock.left90:
            {
                if (this._moveToDirMovement == MovementDirection.right)
                {
                    if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.left)
                {
                    if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.up)
                {
                    if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.down)
                {
                    if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X, roomItem.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.upleft)
                {
                    if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.upright)
                {
                    if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.downright)
                {
                    if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.downleft)
                {
                    if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X + 1, roomItem.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.Room.GameMap.CanStackItem(roomItem.X - 1, roomItem.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                }

                break;
            }
            case WhenMovementBlock.turnback:
            {
                if (this._moveToDirMovement == MovementDirection.right)
                {
                    this._moveToDirMovement = MovementDirection.left;
                }
                else if (this._moveToDirMovement == MovementDirection.left)
                {
                    this._moveToDirMovement = MovementDirection.right;
                }
                else if (this._moveToDirMovement == MovementDirection.up)
                {
                    this._moveToDirMovement = MovementDirection.down;
                }
                else if (this._moveToDirMovement == MovementDirection.down)
                {
                    this._moveToDirMovement = MovementDirection.up;
                }
                else if (this._moveToDirMovement == MovementDirection.upright)
                {
                    this._moveToDirMovement = MovementDirection.downleft;
                }
                else if (this._moveToDirMovement == MovementDirection.downleft)
                {
                    this._moveToDirMovement = MovementDirection.upright;
                }
                else if (this._moveToDirMovement == MovementDirection.upleft)
                {
                    this._moveToDirMovement = MovementDirection.downright;
                }
                else if (this._moveToDirMovement == MovementDirection.downright)
                {
                    this._moveToDirMovement = MovementDirection.upleft;
                }
                break;
            }
            case WhenMovementBlock.turnrandom:
            {
                this._moveToDirMovement = (MovementDirection)WibboEnvironment.GetRandomNumber(1, 7);
                break;
            }
        }

        newPoint = MovementUtility.HandleMovementDir(roomItem.X, roomItem.Y, this._moveToDirMovement);

        if (newPoint != roomItem.Coordinate)
        {
            oldX = disableAnimation ? newPoint.X : roomItem.X;
            oldY = disableAnimation ? newPoint.Y : roomItem.Y;
            oldZ = roomItem.Z;

            if (this.Room.RoomItemHandling.SetFloorItem(null, roomItem, newPoint.X, newPoint.Y, roomItem.Rotation, false, false, false))
            {
                this.Room.SendPacket(new SlideObjectBundleComposer(oldX, oldY, disableAnimation ? roomItem.Z : oldZ, newPoint.X, newPoint.Y, roomItem.Z, roomItem.Id));
            }
        }
        return;
    }

    public void SaveToDatabase(IDbConnection dbClient)
    {
        var startDirection = (MovementDirection)this.GetIntParam(0);
        var whenMoveIsBlocked = (WhenMovementBlock)this.GetIntParam(1);

        WiredUtillity.SaveInDatabase(dbClient, this.Id, Convert.ToInt32(startDirection).ToString(), Convert.ToInt32(whenMoveIsBlocked).ToString(), false, this.Items, this.Delay);
    }

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.Delay = wiredDelay;

        if (int.TryParse(wiredTriggerData2, out var startDirection))
        {
            this.SetIntParam(0, startDirection);
        }

        if (int.TryParse(wiredTriggerData, out var whenMoveIsBlocked))
        {
            this.SetIntParam(1, whenMoveIsBlocked);
        }

        this.LoadStuffIds(wiredTriggersItem);
    }
}
