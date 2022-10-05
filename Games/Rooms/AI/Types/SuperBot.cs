namespace WibboEmulator.Games.Rooms.AI.Types;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Avatar;
using WibboEmulator.Games.GameClients;

public class SuperBot : BotAI
{
    private readonly int _virtualId;
    private int _actionTimer;

    public SuperBot(int virtualId)
    {
        this._virtualId = virtualId;
        this._actionTimer = WibboEnvironment.GetRandomNumber(0, 60);
    }

    public override void OnSelfEnterRoom() => this.GetRoomUser().MoveTo(this.GetRoomUser().X + WibboEnvironment.GetRandomNumber(-10, 10), this.GetRoomUser().Y + WibboEnvironment.GetRandomNumber(-10, 10), true);

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
        if (this.GetBotData() == null)
        {
            return;
        }

        var ownerUser = this.GetRoom().GetRoomUserManager().GetRoomUserByUserId((this.GetBotData().OwnerId == 0) ? this.GetRoom().RoomData.OwnerId : this.GetBotData().OwnerId);
        if (ownerUser == null)
        {
            this.GetRoom().GetRoomUserManager().RemoveBot(this._virtualId, false);

            return;
        }

        if (this._actionTimer <= 0)
        {
            if (this.GetBotData().FollowUser == 0)
            {
                var randomWalkableSquare = this.GetRoom().GetGameMap().GetRandomWalkableSquare(this.GetRoomUser().GoalX, this.GetRoomUser().GoalY);
                this.GetRoomUser().MoveTo(randomWalkableSquare.X, randomWalkableSquare.Y);
            }

            this._actionTimer = WibboEnvironment.GetRandomNumber(10, 60);
        }
        else
        {
            this._actionTimer--;
        }

        if (ownerUser.DanceId != this.GetRoomUser().DanceId)
        {
            this.GetRoomUser().DanceId = ownerUser.DanceId;
            this.GetRoom().SendPacket(new DanceComposer(this.GetRoomUser().VirtualId, this.GetRoomUser().DanceId));
        }
        else if (ownerUser.IsAsleep != this.GetRoomUser().IsAsleep)
        {
            this.GetRoomUser().IsAsleep = ownerUser.IsAsleep;
            this.GetRoom().SendPacket(new SleepComposer(this.GetRoomUser().VirtualId, this.GetRoomUser().IsAsleep));
        }
        else if (ownerUser.CarryItemID != this.GetRoomUser().CarryItemID)
        {
            this.GetRoomUser().CarryItemID = ownerUser.CarryItemID;
            this.GetRoom().SendPacket(new CarryObjectComposer(this.GetRoomUser().VirtualId, this.GetRoomUser().CarryItemID));
        }
        else if (ownerUser.CurrentEffect != this.GetRoomUser().CurrentEffect)
        {
            this.GetRoomUser().CurrentEffect = ownerUser.CurrentEffect;
            this.GetRoom().SendPacket(new AvatarEffectComposer(this.GetRoomUser().VirtualId, this.GetRoomUser().CurrentEffect));
        }

        if (this.GetBotData().FollowUser > 0)
        {
            var user = this.GetRoom().GetRoomUserManager().GetRoomUserByVirtualId(this.GetBotData().FollowUser);
            if (user == null)
            {
                this.GetBotData().FollowUser = 0;
            }
            else
            {
                if (!Gamemap.TilesTouching(this.GetRoomUser().X, this.GetRoomUser().Y, user.X, user.Y))
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

                    this.GetRoomUser().MoveTo(newX, newY, true);
                }
            }
        }
    }
}
