using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Items.Wired.Interfaces;
using WibboEmulator.Game.Rooms;
using System.Data;

namespace WibboEmulator.Game.Items.Wired.Actions
{
    public class BotClothes : WiredActionBase, IWired, IWiredEffect, IWiredCycleable
    {
        public BotClothes(Item item, Room room) : base(item, room, (int)WiredActionType.BOT_CHANGE_FIGURE)
        {
        }

        public override bool OnCycle(RoomUser user, Item item)
        {
            if (string.IsNullOrWhiteSpace(this.StringParam) || !this.StringParam.Contains("\t"))
            {
                return false;
            }

            string[] nameAndLook = this.StringParam.Split('\t');
            string nameBot = (nameAndLook.Length == 2) ? nameAndLook[0] : "";
            string look = (nameAndLook.Length == 2) ? nameAndLook[1] : "";

            if (nameBot == "" || look == "")
            {
                return false;
            }

            RoomUser Bot = this.RoomInstance.GetRoomUserManager().GetBotOrPetByName(nameBot);
            if (Bot == null)
            {
                return false;
            }

            Bot.BotData.Look = look;

            this.RoomInstance.SendPacket(new UserChangeComposer(Bot));

            return false;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, this.StringParam, false, null, this.Delay);
        }

        public void LoadFromDatabase(DataRow row)
        {
            if (int.TryParse(row["delay"].ToString(), out int delay))
	            this.Delay = delay;

            string triggerData = row["trigger_data"].ToString();

            if (string.IsNullOrWhiteSpace(triggerData) || !triggerData.Contains("\t"))
            {
                return;
            }

            this.StringParam = triggerData;
        }
    }
}
