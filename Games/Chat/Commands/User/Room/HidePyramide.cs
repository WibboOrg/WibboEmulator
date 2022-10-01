using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class HidePyramide : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            foreach (Item Item in Room.GetRoomItemHandler().GetFloor.ToList())
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

            Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.pyramide", Session.Langue));
        }
    }
}
