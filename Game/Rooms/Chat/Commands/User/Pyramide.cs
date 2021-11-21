using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using System.Linq;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd{    internal class Pyramide : IChatCommand    {        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)        {
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
                Item.GetRoom().GetGameMap().updateMapForItem(Item);
            }            UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.pyramide", Session.Langue));        }    }}