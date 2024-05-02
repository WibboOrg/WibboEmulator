namespace WibboEmulator.Games.Rooms.AI.Types;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms.Map;

public class BartenderBot : BotAI
{
    private int _speechTimer;
    private int _actionTimer;

    public BartenderBot(int virtualId)
    {
        this.VirtualId = virtualId;

        this._speechTimer = WibboEnvironment.GetRandomNumber(10, 40);
        this._actionTimer = WibboEnvironment.GetRandomNumber(10, 30);
    }

    public override void OnSelfEnterRoom()
    {
    }

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

        if (this._speechTimer <= 0)
        {
            if (this.BotData.RandomSpeech.Count > 0 && this.BotData.AutomaticChat)
            {
                this.RoomUser.OnChat(this.BotData.GetRandomSpeech(), 2, false);
            }

            this._speechTimer = this.BotData.SpeakingInterval * 2;
        }
        else
        {
            this._speechTimer--;
        }

        if (this._actionTimer <= 0)
        {
            if (this.BotData.WalkingEnabled && this.BotData.FollowUser == 0)
            {
                var randomWalkableSquare = this.Room.GameMap.GetRandomWalkableSquare(this.BotData.X, this.BotData.Y);
                this.RoomUser.MoveTo(randomWalkableSquare.X, randomWalkableSquare.Y);
            }
            this._actionTimer = WibboEnvironment.GetRandomNumber(10, 60);
        }
        else
        {
            this._actionTimer--;
        }

        if (this.BotData.FollowUser != 0)
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
                    this.RoomUser.MoveTo(user.X, user.Y, true);
                }
            }
        }
    }
}
