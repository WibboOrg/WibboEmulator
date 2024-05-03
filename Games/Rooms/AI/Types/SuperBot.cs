namespace WibboEmulator.Games.Rooms.AI.Types;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Avatar;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms.Map;

public class SuperBot : BotAI
{
    private int _actionTimer;

    public SuperBot(int virtualId)
    {
        this.VirtualId = virtualId;

        this._actionTimer = WibboEnvironment.GetRandomNumber(0, 60);
    }

    public override void OnSelfEnterRoom() => this.RoomUser.MoveTo(this.RoomUser.X + WibboEnvironment.GetRandomNumber(-10, 10), this.RoomUser.Y + WibboEnvironment.GetRandomNumber(-10, 10), true);

    public override void OnSelfLeaveRoom(bool kicked)
    {
    }

    public override void OnUserEnterRoom(RoomUser user)
    {
    }

    public override void OnUserLeaveRoom(GameClient client)
    {
    }

    public override void OnUserSay(RoomUser user, string message)
    {
    }

    public override void OnUserShout(RoomUser user, string message)
    {
    }

    public override void OnTimerTick()
    {
        if (this.BotData == null)
        {
            return;
        }

        var ownerUser = this.Room.RoomUserManager.GetRoomUserByUserId((this.BotData.OwnerId == 0) ? this.Room.RoomData.OwnerId : this.BotData.OwnerId);
        if (ownerUser == null)
        {
            this.Room.RoomUserManager.RemoveBot(this.VirtualId, false);

            return;
        }

        if (this._actionTimer <= 0)
        {
            if (this.BotData.FollowUser == 0)
            {
                var randomWalkableSquare = this.Room.GameMap.GetRandomWalkableSquare(this.RoomUser.GoalX, this.RoomUser.GoalY);
                this.RoomUser.MoveTo(randomWalkableSquare.X, randomWalkableSquare.Y);
            }

            this._actionTimer = WibboEnvironment.GetRandomNumber(10, 60);
        }
        else
        {
            this._actionTimer--;
        }

        if (ownerUser.DanceId != this.RoomUser.DanceId)
        {
            this.RoomUser.DanceId = ownerUser.DanceId;
            this.Room.SendPacket(new DanceComposer(this.RoomUser.VirtualId, this.RoomUser.DanceId));
        }
        else if (ownerUser.IsAsleep != this.RoomUser.IsAsleep)
        {
            this.RoomUser.IsAsleep = ownerUser.IsAsleep;
            this.Room.SendPacket(new SleepComposer(this.RoomUser.VirtualId, this.RoomUser.IsAsleep));
        }
        else if (ownerUser.CarryItemID != this.RoomUser.CarryItemID)
        {
            this.RoomUser.CarryItemID = ownerUser.CarryItemID;
            this.Room.SendPacket(new CarryObjectComposer(this.RoomUser.VirtualId, this.RoomUser.CarryItemID));
        }
        else if (ownerUser.CurrentEffect != this.RoomUser.CurrentEffect)
        {
            this.RoomUser.CurrentEffect = ownerUser.CurrentEffect;
            this.Room.SendPacket(new AvatarEffectComposer(this.RoomUser.VirtualId, this.RoomUser.CurrentEffect));
        }

        if (this.BotData.FollowUser > 0)
        {
            var user = this.Room.RoomUserManager.GetRoomUserByVirtualId(this.BotData.FollowUser);
            if (user == null)
            {
                this.BotData.FollowUser = 0;
            }
            else
            {
                if (!GameMap.TilesTouching(this.RoomUser.X, this.RoomUser.Y, user.X, user.Y))
                {
                    var newX = user.X;
                    var newY = user.Y;

                    switch (WibboEnvironment.GetRandomNumber(1, 3))
                    {
                        case 1:
                            newY--;
                            break;
                        case 2:
                            newY++;
                            break;
                        case 3:
                            break;
                    }

                    switch (WibboEnvironment.GetRandomNumber(1, 3))
                    {
                        case 1:
                            newX--;
                            break;
                        case 2:
                            newX++;
                            break;
                        case 3:
                            break;
                    }

                    this.
                    RoomUser.MoveTo(newX, newY, true);
                }
            }
        }
    }
}
