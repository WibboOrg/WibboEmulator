namespace WibboEmulator.Games.Rooms.AI;
using WibboEmulator.Games.GameClients;

public abstract class BotAI
{
    public int Id { get; set; }
    private RoomUser _roomUser;
    private Room _room;

    public BotAI()
    {
    }

    public void Init(int baseId, RoomUser user, Room room)
    {
        this.Id = baseId;
        this._roomUser = user;
        this._room = room;
    }

    public Room GetRoom() => this._room;

    public RoomUser GetRoomUser() => this._roomUser;

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

    public abstract void OnSelfLeaveRoom(bool kicked);

    public abstract void OnUserEnterRoom(RoomUser user);

    public abstract void OnUserLeaveRoom(GameClient client);

    public abstract void OnUserSay(RoomUser user, string message);

    public abstract void OnUserShout(RoomUser user, string message);

    public abstract void OnTimerTick();
}
