using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Users.Wardrobes;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Communication.Packets.Outgoing.Avatar
{
    internal class WardrobeComposer : ServerPacket
    {
        public WardrobeComposer(List<Wardrobe> wardrobes)
            : base(ServerPacketHeader.USER_OUTFITS)
        {
            this.WriteInteger(1);

            this.WriteInteger(wardrobes.Count);
            foreach (Wardrobe wardrobe in wardrobes)
            {
                this.WriteInteger(wardrobe.SlotId);
                this.WriteString(wardrobe.Look);
                this.WriteString(wardrobe.Gender);
            }
        }
    }
}