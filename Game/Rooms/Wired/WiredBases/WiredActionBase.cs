using Butterfly.Communication.Packets.Outgoing.Rooms.Wireds;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using System.Collections.Generic;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers
{
    public class WiredActionBase : WiredBase
    {
        public int DelayCycle { get => this.Delay; }

        internal WiredActionBase(Item item, Room room, int type) : base(item, room, type)
        {
        }

        public override void OnTrigger(Client Session)
        {
            Session.SendPacket(new WiredFurniActionMessageComposer(this.StuffTypeSelectionEnabled, this.FurniLimit, this.StuffIds, this.StuffTypeId, this.Id,
                this.StringParam, this.IntParams, this.StuffTypeSelectionCode, this.Type, this.Delay, this.Conflicting));
        }
    }
}
