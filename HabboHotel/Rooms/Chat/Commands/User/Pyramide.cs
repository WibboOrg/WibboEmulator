using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using System.Linq;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    internal class Pyramide : IChatCommand    {        public string PermissionRequired
        {
            get { return ""; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return ""; }
        }        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)        {
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