namespace WibboEmulator.Games.Rooms.AI.Types;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms.Map;

public class BartenderBot : BotAI
{
    private int _speechTimer;
    private int _actionTimer;

    public BartenderBot(int virtualId)
    {
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
        if (this.GetBotData() == null)
        {
            return;
        }

        if (this._speechTimer <= 0)
        {
            if (this.GetBotData().RandomSpeech.Count > 0 && this.GetBotData().AutomaticChat)
            {
                this.GetRoomUser().OnChat(this.GetBotData().GetRandomSpeech(), 2, false);
            }

            this._speechTimer = this.GetBotData().SpeakingInterval * 2;
        }
        else
        {
            this._speechTimer--;
        }

        if (this._actionTimer <= 0)
        {
            if (this.GetBotData().WalkingEnabled && this.GetBotData().FollowUser == 0)
            {
                var randomWalkableSquare = this.GetRoom().GetGameMap().GetRandomWalkableSquare(this.GetBotData().X, this.GetBotData().Y);
                this.GetRoomUser().MoveTo(randomWalkableSquare.X, randomWalkableSquare.Y);
            }
            this._actionTimer = WibboEnvironment.GetRandomNumber(10, 60);
        }
        else
        {
            this._actionTimer--;
        }

        if (this.GetBotData().FollowUser != 0)
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
                    this.GetRoomUser().MoveTo(user.X, user.Y, true);
                }
            }
        }
    }
}
