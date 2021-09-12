using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.AI.Types
{
    public class RoleplayBot : BotAI
    {
        private readonly int SpeechTimer;
        private readonly int ActionTimer;

        public RoleplayBot(int VirtualId)
        {
            this.SpeechTimer = ButterflyEnvironment.GetRandomNumber(10, 40);
            this.ActionTimer = ButterflyEnvironment.GetRandomNumber(10, 30);
        }

        public override void OnSelfEnterRoom()
        {
        }

        public override void OnSelfLeaveRoom(bool Kicked)
        {
        }

        public override void OnUserEnterRoom(RoomUser User)
        {
        }

        public override void OnUserLeaveRoom(GameClient Client)
        {
        }

        public override void OnUserSay(RoomUser User, string Message)
        {
        }

        public override void OnUserShout(RoomUser User, string Message)
        {
        }

        public override void OnTimerTick()
        {
        }
    }
}
