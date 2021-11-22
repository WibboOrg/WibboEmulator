using Butterfly.Communication.Packets.Outgoing.Rooms.Wireds;
using Butterfly.Game.Clients;
using System.Collections.Generic;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers
{
    public class WiredBase
    {
        internal bool StuffTypeSelectionEnabled;
        internal int FurniLimit;
        internal List<int> StuffIds;
        internal int StuffTypeId;
        internal int Id;
        internal string StringParam;
        internal List<int> IntParams;
        internal int StuffTypeSelectionCode;
        internal int Type;
        internal List<int> Conflicting;
        internal int DelayInPulses;

        internal bool isDisposed;
        internal WiredBase()
        {
            this.StuffTypeSelectionEnabled = false;
            this.FurniLimit = 20;
            this.StuffIds = new List<int>();
            this.StuffTypeId = 0;
            this.Id = 0;
            this.StringParam = "";
            this.IntParams = new List<int>();
            this.StuffTypeSelectionCode = 0;
            this.Type = 0;
            this.Conflicting = new List<int>();
            this.DelayInPulses = 0;
        }
        internal virtual void SendWiredPacket(Client Session)
        {
            
        }

        public void Dispose()
        {
            this.isDisposed = true;
        }

        public bool Disposed()
        {
            return this.isDisposed;
        }
    }
}
