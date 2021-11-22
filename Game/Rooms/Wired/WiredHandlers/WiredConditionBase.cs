using Butterfly.Communication.Packets.Outgoing.Rooms.Wireds;
using Butterfly.Game.Clients;
using System.Collections.Generic;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers
{
    public class WiredConditionBase
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

        internal bool isDisposed;
        internal WiredConditionBase()
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
        }
        public void SendWiredPacket(Client Session)
        {
            //new WiredFurniConditionMessageComposer(false, 0, null, SpriteId, this.Id, "", new List<int> { (int)this.team }, 0, (int)WiredConditionType.ACTOR_IS_IN_TEAM)
            Session.SendPacket(new WiredFurniConditionMessageComposer(this.StuffTypeSelectionEnabled, this.FurniLimit, this.StuffIds, this.StuffTypeId, this.Id,
                this.StringParam, this.IntParams, this.StuffTypeSelectionCode, this.Type));
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
