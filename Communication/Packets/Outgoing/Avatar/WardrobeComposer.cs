using Butterfly.Game.Users.Wardrobes;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Outgoing.Avatar
{
    internal class WardrobeComposer : ServerPacket
    {
        public WardrobeComposer(Dictionary<int, Wardrobe> wardrobes)
            : base(ServerPacketHeader.USER_OUTFITS)
        {
            this.WriteInteger(1);

            this.WriteInteger(wardrobes.Count);
            foreach (Wardrobe wardrobe in wardrobes.Values)
            {
                this.WriteInteger(wardrobe.SlotId);
                this.WriteString(wardrobe.Look);
                this.WriteString(wardrobe.Gender);
            }
        }
    }
}