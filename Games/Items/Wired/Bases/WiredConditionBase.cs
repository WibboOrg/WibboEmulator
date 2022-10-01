using WibboEmulator.Communication.Packets.Outgoing.Rooms.Wireds;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Items.Wired
{
    public class WiredConditionBase : WiredBase
    {
        internal WiredConditionBase(Item item, Room room, int type) : base(item, room, type)
        {
            
        }

        public override void OnTrigger(GameClient Session)
        {
            Session.SendPacket(new WiredFurniConditionComposer(this.StuffTypeSelectionEnabled, this.FurniLimit, this.StuffIds, this.StuffTypeId, this.Id,
                this.StringParam, this.IntParams, this.StuffTypeSelectionCode, this.Type));
        }
    }
}
