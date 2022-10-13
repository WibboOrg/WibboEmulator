namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Interfaces;
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

        this.IntParams.Add(0);
        this.IntParams.Add(0);
    }

    public override bool OnCycle(RoomUser user, Item item)
    {
        foreach (var roomItem in this.Items.ToList())
        {
            this.HandleMovement(roomItem);
        }

        return false;
    }

    public override void LoadItems(bool inDatabase = false)
    {
        base.LoadItems();

        var startDirection = (MovementDirection)((this.IntParams.Count > 0) ? this.IntParams[0] : 0);

        this._moveToDirMovement = startDirection;
    }

    private void HandleMovement(Item item)
    {
        if (this.RoomInstance.RoomItemHandling.GetItem(item.Id) == null)
        {
            return;
        }

        var newPoint = MovementUtility.HandleMovementDir(item.X, item.Y, this._moveToDirMovement);

        var roomUser = this.RoomInstance.RoomUserManager.GetUserForSquare(newPoint.X, newPoint.Y);
        if (roomUser != null)
        {
            this.RoomInstance.WiredHandler.TriggerCollision(roomUser, item);
            return;
        }

        //var startDirection = (MovementDirection)((this.IntParams.Count > 0) ? this.IntParams[0] : 0);
        var whenMoveIsBlocked = (WhenMovementBlock)((this.IntParams.Count > 1) ? this.IntParams[1] : 0);

        var oldX = item.X;
        var oldY = item.Y;
        var oldZ = item.Z;
        if (this.RoomInstance.RoomItemHandling.SetFloorItem(null, item, newPoint.X, newPoint.Y, item.Rotation, false, false, false))
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
                if (this._moveToDirMovement == MovementDirection.right)
                {
                    if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.left)
                {
                    if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.up)
                {
                    if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.down)
                {
                    if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.upleft)
                {
                    if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.upright)
                {
                    if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    return;
                }
                else if (this._moveToDirMovement == MovementDirection.downright)
                {
                    if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.downleft)
                {
                    if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
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
                    if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.left)
                {
                    if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.up)
                {
                    if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.down)
                {
                    if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.upleft)
                {
                    if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.upright)
                {
                    if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    return;
                }
                else if (this._moveToDirMovement == MovementDirection.downright)
                {
                    if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.downleft)
                {
                    if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
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
                    if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.left)
                {
                    if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.up)
                {
                    if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.down)
                {
                    if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.upleft)
                {
                    if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.upright)
                {
                    if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.downright)
                {
                    if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.downleft)
                {
                    if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
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
                    if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.left)
                {
                    if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.up)
                {
                    if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.down)
                {
                    if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y)) // derecha
                    {
                        this._moveToDirMovement = MovementDirection.right;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y - 1)) // arriba
                    {
                        this._moveToDirMovement = MovementDirection.up;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y)) // izq
                    {
                        this._moveToDirMovement = MovementDirection.left;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X, item.Y + 1)) // abajo
                    {
                        this._moveToDirMovement = MovementDirection.down;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.upleft)
                {
                    if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.upright)
                {
                    if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.downright)
                {
                    if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
                    {
                        this._moveToDirMovement = MovementDirection.downleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                }
                else if (this._moveToDirMovement == MovementDirection.downleft)
                {
                    if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y + 1)) // abajo derecha
                    {
                        this._moveToDirMovement = MovementDirection.downright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X + 1, item.Y - 1)) // arriba derecha
                    {
                        this._moveToDirMovement = MovementDirection.upright;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y - 1)) // arriba izq
                    {
                        this._moveToDirMovement = MovementDirection.upleft;
                        break;
                    }
                    else if (this.RoomInstance.GameMap.CanStackItem(item.X - 1, item.Y + 1)) // abajo izq
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

        newPoint = MovementUtility.HandleMovementDir(item.X, item.Y, this._moveToDirMovement);

        if (newPoint != item.Coordinate)
        {
            oldX = item.X;
            oldY = item.Y;
            oldZ = item.Z;

            if (this.RoomInstance.RoomItemHandling.SetFloorItem(null, item, newPoint.X, newPoint.Y, item.Rotation, false, false, false))
            {
                this.RoomInstance.SendPacket(new SlideObjectBundleComposer(oldX, oldY, oldZ, newPoint.X, newPoint.Y, item.Z, item.Id));
            }
        }
        return;
    }

    public void SaveToDatabase(IQueryAdapter dbClient)
    {
        var startDirection = (MovementDirection)((this.IntParams.Count > 0) ? this.IntParams[0] : 0);
        var whenMoveIsBlocked = (WhenMovementBlock)((this.IntParams.Count > 1) ? this.IntParams[1] : 0);

        WiredUtillity.SaveTriggerItem(dbClient, this.Id, Convert.ToInt32(startDirection).ToString(), Convert.ToInt32(whenMoveIsBlocked).ToString(), false, this.Items, this.Delay);
    }

    public void LoadFromDatabase(DataRow row)
    {
        this.IntParams.Clear();

        if (int.TryParse(row["delay"].ToString(), out var delay))
        {
            this.Delay = delay;
        }

        var triggerItems = row["triggers_item"].ToString();

        if (int.TryParse(row["trigger_data_2"].ToString(), out var startDirection))
        {
            this.IntParams.Add(startDirection);
        }

        if (int.TryParse(row["trigger_data"].ToString(), out var whenMoveIsBlocked))
        {
            this.IntParams.Add(whenMoveIsBlocked);
        }

        if (triggerItems is null or "")
        {
            return;
        }

        foreach (var itemId in triggerItems.Split(';'))
        {
            if (!int.TryParse(itemId, out var id))
            {
                continue;
            }

            if (!this.StuffIds.Contains(id))
            {
                this.StuffIds.Add(id);
            }
        }
    }
}
