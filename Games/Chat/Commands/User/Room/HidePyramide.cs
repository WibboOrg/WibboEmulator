namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

internal class HidePyramide : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        foreach (var Item in Room.GetRoomItemHandler().GetFloor.ToList())
        {
            if (Item == null || Item.GetBaseItem() == null)
            {
                continue;
            }

            if (Item.GetBaseItem().ItemName != "wf_pyramid")
            {
                continue;
            }

            Item.ExtraData = (Item.ExtraData == "0") ? "1" : "0";
            Item.UpdateState();
            Item.GetRoom().GetGameMap().UpdateMapForItem(Item);
        }

        session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.pyramide", session.Langue));
    }
}
