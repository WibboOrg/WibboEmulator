using Butterfly.Communication.Packets.Outgoing.Rooms.Wireds;
using Butterfly.Game.Clients;
using System.Collections.Generic;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers
{
    public class WiredActionBase : WiredBase
    {

        internal WiredActionBase() : base()
        {
        }
        internal override void SendWiredPacket(Client Session)
        {
            Session.SendPacket(new WiredFurniActionMessageComposer(this.StuffTypeSelectionEnabled, this.FurniLimit, this.StuffIds, this.StuffTypeId, this.Id,
                this.StringParam, this.IntParams, this.StuffTypeSelectionCode, this.Type, this.DelayInPulses, this.Conflicting));
        }
    }
}
