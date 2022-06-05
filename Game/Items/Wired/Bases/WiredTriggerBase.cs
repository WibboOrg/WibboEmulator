using Wibbo.Communication.Packets.Outgoing.Rooms.Wireds;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Items.Wired
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
