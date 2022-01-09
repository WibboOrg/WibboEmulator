using Butterfly.Game.Clients;

namespace Butterfly.Game.Rooms.AI
{
    public abstract class BotAI
    {
        public int Id { get; set; }
        private RoomUser roomUser;
        private Room room;

        public BotAI()
        {
        }

        public void Init(int baseId, RoomUser user, Room room)
        {
            this.Id = baseId;
            this.roomUser = user;
            this.room = room;
        }

        public Room GetRoom()
        {
            return this.room;
        }

        public RoomUser GetRoomUser()
        {
            return this.roomUser;
        }

        public RoomBot GetBotData()
        {
            if (this.GetRoomUser() == null)
            {
                return null;
            }
            else
            {
                return this.GetRoomUser().BotData;
            }
        }

        public abstract void OnSelfEnterRoom();

        public abstract void OnSelfLeaveRoom(bool Kicked);

        public abstract void OnUserEnterRoom(RoomUser User);

        public abstract void OnUserLeaveRoom(Client Client);

        public abstract void OnUserSay(RoomUser User, string Message);

        public abstract void OnUserShout(RoomUser User, string Message);

        public abstract void OnTimerTick();
    }
}
