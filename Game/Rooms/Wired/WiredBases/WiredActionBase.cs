using Butterfly.Communication.Packets.Outgoing.Rooms.Wireds;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers
{
    public class WiredActionBase : WiredBase, IWiredCycleable
    {
        public int DelayCycle { get => this.Delay; }

        public virtual void Handle(RoomUser user, Item item)
        {
            if (this.DelayCycle > 0)
            {
                this.RoomInstance.GetWiredHandler().RequestCycle(new WiredCycle(this, user, item, this.DelayCycle));
            }
            else
            {
                this.OnCycle(user, item);
            }
        }

        public virtual bool OnCycle(RoomUser user, Item item)
        {
            return false;
        }

        internal WiredActionBase(Item item, Room room, int type) : base(item, room, type)
        {
        }

        public override void OnTrigger(Client Session)
        {
            Session.SendPacket(new WiredFurniActionComposer(this.StuffTypeSelectionEnabled, this.FurniLimit, this.StuffIds, this.StuffTypeId, this.Id,
                this.StringParam, this.IntParams, this.StuffTypeSelectionCode, this.Type, this.Delay, this.Conflicting));
        }
    }
}
