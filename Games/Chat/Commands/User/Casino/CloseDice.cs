namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

internal class CloseDice : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        var userBooth = Room.GetRoomItemHandler().GetFloor.Where(x => x != null && Gamemap.TilesTouching(
            x.X, x.Y, UserRoom.X, UserRoom.Y) && x.Data.InteractionType == InteractionType.DICE).ToList();

        if (userBooth == null)
        {
            return;
        }

        UserRoom.DiceCounterAmount = 0;
        UserRoom.DiceCounter = 0;

        userBooth.ForEach(x =>
        {
            x.ExtraData = "0";
            x.UpdateState();
        });
    }
}
