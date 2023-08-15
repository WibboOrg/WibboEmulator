namespace WibboEmulator.Games.Chats.Commands.User.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class HidePyramide : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        foreach (var item in room.RoomItemHandling.GetFloor.ToList())
        {
            if (item == null || item.GetBaseItem() == null)
            {
                continue;
            }

            if (item.GetBaseItem().ItemName is not "wf_pyramid" and not "pyra_rose" and not "pyra_rouge" and not "pyra_violette" and not "pyra_exit" and not "bb_pyramid" and not "pyramide_exit")
            {
                continue;
            }

            item.ExtraData = item.ExtraData == "0" ? "1" : "0";
            item.UpdateState();
            item.GetRoom().GameMap.UpdateMapForItem(item);
        }

        session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.pyramide", session.Langue));
    }
}
