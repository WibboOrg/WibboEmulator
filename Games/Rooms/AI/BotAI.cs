namespace WibboEmulator.Games.Rooms.AI;
using WibboEmulator.Games.GameClients;

public abstract class BotAI
{
    public int Id { get; set; }
    public int VirtualId { get; set; }
    public RoomUser RoomUser { get; private set; }
    public Room Room { get; private set; }

    public BotAI()
    {
    }

    public virtual void Initialize(int baseId, RoomUser user, Room room)
    {
        this.Id = baseId;
        this.RoomUser = user;
        this.Room = room;
    }

    public RoomBot BotData
    {
        get
        {
            if (this.RoomUser == null)
            {
                return null;
            }
            else
            {
                return this.RoomUser.BotData;
            }
        }
    }

    public abstract void OnSelfEnterRoom();

    public abstract void OnSelfLeaveRoom(bool kicked);

    public abstract void OnUserEnterRoom(RoomUser user);

    public abstract void OnUserLeaveRoom(GameClient client);

    public abstract void OnUserSay(RoomUser user, string message);

    public abstract void OnUserShout(RoomUser user, string message);

    public abstract void OnTimerTick();
}
