using WibboEmulator.Communication.Packets.Outgoing.Rooms.Wireds;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Items.Wired
{
    public class WiredConditionBase : WiredBase
    {
        internal WiredConditionBase(Item item, Room room, int type) : base(item, room, type)
        {
            
        }

        public override void OnTrigger(Client Session)
        {
            Session.SendPacket(new WiredFurniConditionComposer(this.StuffTypeSelectionEnabled, this.FurniLimit, this.StuffIds, this.StuffTypeId, this.Id,
                this.StringParam, this.IntParams, this.StuffTypeSelectionCode, this.Type));
        }
    }
}
