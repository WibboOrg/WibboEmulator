namespace WibboEmulator.Games.Rooms.Map.Movement;
using System.Drawing;

public static class MovementUtility
{
    public static void HandleMovement(ref Point coordinate, MovementState state)
    {
        switch (state)
        {
            case MovementState.up:
                coordinate.Y--;
                break;
            case MovementState.right:
                coordinate.X++;
                break;
            case MovementState.down:
                coordinate.Y++;
                break;
            case MovementState.left:
                coordinate.X--;
                break;
        }
    }

    public static Point HandleMovement(Point newCoordinate, MovementState state)
    {
        var coordinate = new Point(newCoordinate.X, newCoordinate.Y);
        switch (state)
        {
            case MovementState.random:
                switch (WibboEnvironment.GetRandomNumber(1, 4))
                {
                    case 1:
                        HandleMovement(ref coordinate, MovementState.up);
                        break;
                    case 2:
                        HandleMovement(ref coordinate, MovementState.down);
                        break;
                    case 3:
                        HandleMovement(ref coordinate, MovementState.left);
                        break;
                    case 4:
                        HandleMovement(ref coordinate, MovementState.right);
                        break;
                }
                break;
            case MovementState.leftright:
                if (WibboEnvironment.GetRandomNumber(0, 1) == 1)
                {
                    HandleMovement(ref coordinate, MovementState.left);
                    break;
                }
                else
                {
                    HandleMovement(ref coordinate, MovementState.right);
                    break;
                }
            case MovementState.updown:
                if (WibboEnvironment.GetRandomNumber(0, 1) == 1)
                {
                    HandleMovement(ref coordinate, MovementState.up);
                    break;
                }
                else
                {
                    HandleMovement(ref coordinate, MovementState.down);
                    break;
                }
            case MovementState.up:
            case MovementState.right:
            case MovementState.down:
            case MovementState.left:
                HandleMovement(ref coordinate, state);
                break;
        }
        return coordinate;
    }

    public static void HandleMovementDir(ref Point coordinate, MovementDirection state)
    {
        switch (state)
        {
            case MovementDirection.down:
            {
                coordinate.Y++;
                break;
            }

            case MovementDirection.up:
            {
                coordinate.Y--;
                break;
            }

            case MovementDirection.left:
            {
                coordinate.X--;
                break;
            }

            case MovementDirection.right:
            {
                coordinate.X++;
                break;
            }

            case MovementDirection.downright:
            {
                coordinate.X++;
                coordinate.Y++;
                break;
            }

            case MovementDirection.downleft:
            {
                coordinate.X--;
                coordinate.Y++;
                break;
            }

            case MovementDirection.upright:
            {
                coordinate.X++;
                coordinate.Y--;
                break;
            }

            case MovementDirection.upleft:
            {
                coordinate.X--;
                coordinate.Y--;
                break;
            }
        }
    }

    public static Point HandleMovementDir(int x, int y, MovementDirection state)
    {
        var newPoint = new Point(x, y);

        switch (state)
        {
            case MovementDirection.up:
            case MovementDirection.down:
            case MovementDirection.left:
            case MovementDirection.right:
            case MovementDirection.downright:
            case MovementDirection.downleft:
            case MovementDirection.upright:
            case MovementDirection.upleft:
            {
                HandleMovementDir(ref newPoint, state);
                break;
            }

            case MovementDirection.random:
            {
                switch (WibboEnvironment.GetRandomNumber(1, 4))
                {
                    case 1:
                    {
                        HandleMovementDir(ref newPoint, MovementDirection.up);
                        break;
                    }
                    case 2:
                    {
                        HandleMovementDir(ref newPoint, MovementDirection.down);
                        break;
                    }

                    case 3:
                    {
                        HandleMovementDir(ref newPoint, MovementDirection.left);
                        break;
                    }
                    case 4:
                    {
                        HandleMovementDir(ref newPoint, MovementDirection.right);
                        break;
                    }
                }
                break;
            }
        }

        return newPoint;
    }

    public static MovementDirection GetMovementByDirection(int rot)
    {
        var movement = MovementDirection.none;

        switch (rot)
        {
            case 0:
                movement = MovementDirection.up;
                break;
            case 1:
                movement = MovementDirection.upright;
                break;
            case 2:
                movement = MovementDirection.right;
                break;
            case 3:
                movement = MovementDirection.downright;
                break;
            case 4:
                movement = MovementDirection.down;
                break;
            case 5:
                movement = MovementDirection.downleft;
                break;
            case 6:
                movement = MovementDirection.left;
                break;
            case 7:
                movement = MovementDirection.upleft;
                break;
        }

        return movement;
    }

    public static Point GetMoveCoord(int x, int y, int i, MovementDirection movementDir)
    {
        switch (movementDir)
        {
            case MovementDirection.up:
            {
                y -= i;
                break;
            }
            case MovementDirection.upright:
            {
                x += i;
                y -= i;
                break;
            }
            case MovementDirection.right:
            {
                x += i;
                break;
            }
            case MovementDirection.downright:
            {
                x += i;
                y += i;
                break;
            }
            case MovementDirection.down:
            {
                y += i;
                break;
            }
            case MovementDirection.downleft:
            {
                x -= i;
                y += i;
                break;
            }
            case MovementDirection.left:
            {
                x -= i;
                break;
            }
            case MovementDirection.upleft:
            {
                x -= i;
                y -= i;
                break;
            }
        }

        return new Point(x, y);
    }

    public static int HandleRotation(int oldRotation, RotationState state)
    {
        var rotation = oldRotation;
        switch (state)
        {
            case RotationState.CLOCWISE:
                HandleClockwiseRotation(ref rotation);
                return rotation;
            case RotationState.COUNTERCLOCKWISE:
                HandleCounterClockwiseRotation(ref rotation);
                return rotation;
            case RotationState.RANDOM:
                if (WibboEnvironment.GetRandomNumber(0, 1) == 1)
                {
                    HandleClockwiseRotation(ref rotation);
                }
                else
                {
                    HandleCounterClockwiseRotation(ref rotation);
                }

                return rotation;
            default:
                return rotation;
        }
    }

    public static void HandleClockwiseRotation(ref int rotation)
    {
        rotation += 2;
        if (rotation <= 6)
        {
            return;
        }

        rotation = 0;
    }

    public static void HandleCounterClockwiseRotation(ref int rotation)
    {
        rotation -= 2;
        if (rotation >= 0)
        {
            return;
        }

        rotation = 6;
    }
}
public enum RotationState
{
    NONE,
    CLOCWISE,
    COUNTERCLOCKWISE,
    RANDOM,
}
