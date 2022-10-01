using WibboEmulator.Communication.Packets.Outgoing.Rooms.Wireds;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Items.Wired
{
    public class WiredTriggerBase : WiredBase
    {
        internal WiredTriggerBase(Item item, Room room, int type) : base(item, room, type)
        {
            
        }

        public override void OnTrigger(Client Session)
        {
            Session.SendPacket(new WiredFurniTriggerComposer(this.StuffTypeSelectionEnabled, this.FurniLimit, this.StuffIds, this.StuffTypeId, this.Id,
                this.StringParam, this.IntParams, this.StuffTypeSelectionCode, this.Type, this.Conflicting));
        }
    }
}
