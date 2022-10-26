namespace WibboEmulator.Games.Chat.Commands.User.Casino;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Map;

internal class CloseDice : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var userBooth = room.RoomItemHandling.GetFloor.Where(x => x != null && GameMap.TilesTouching(
            x.X, x.Y, userRoom.X, userRoom.Y) && x.Data.InteractionType == InteractionType.DICE).ToList();

        if (userBooth == null)
        {
            return;
        }

        userRoom.DiceCounterAmount = 0;
        userRoom.DiceCounter = 0;

        userBooth.ForEach(x =>
        {
            x.ExtraData = "0";
            x.UpdateState();
        });
    }
}
