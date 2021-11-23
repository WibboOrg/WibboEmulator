using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Actions
{
    public class BotClothes : WiredActionBase, IWired, IWiredEffect
    {
        public BotClothes(Item item, Room room) : base(item, room, (int)WiredActionType.BOT_CHANGE_FIGURE)
        {
        }

        public void Handle(RoomUser user, Item item)
        {
            if (string.IsNullOrWhiteSpace(this.StringParam) || !this.StringParam.Contains("\t"))
            {
                return;
            }

            string[] nameAndLook = this.StringParam.Split('\t');
            string nameBot = (nameAndLook.Length == 2) ? nameAndLook[0] : "";
            string look = (nameAndLook.Length == 2) ? nameAndLook[1] : "";

            if (nameBot == "" || look == "")
            {
                return;
            }

            RoomUser Bot = this.RoomInstance.GetRoomUserManager().GetBotOrPetByName(nameBot);
            if (Bot == null)
            {
                return;
            }

            Bot.BotData.Look = look;

            this.RoomInstance.SendPacket(new UserChangeComposer(Bot));
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, this.StringParam, false, null);
        }

        public void LoadFromDatabase(DataRow row)
        {
            string triggerData = row["trigger_data"].ToString();

            if (string.IsNullOrWhiteSpace(triggerData) || !triggerData.Contains("\t"))
            {
                return;
            }

            this.StringParam = triggerData;
        }
    }
}
