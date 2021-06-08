using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms.Wired;

namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class ObjectUpdateComposer : ServerPacket
    {
        public ObjectUpdateComposer(Item Item, int UserId, bool HideWired = false)
            : base(ServerPacketHeader.FURNITURE_FLOOR_UPDATE)
        {
            this.WriteInteger(Item.Id);
            this.WriteInteger((HideWired && WiredUtillity.TypeIsWired(Item.GetBaseItem().InteractionType) && (Item.GetBaseItem().InteractionType != InteractionType.HIGHSCORE && Item.GetBaseItem().InteractionType != InteractionType.HIGHSCOREPOINTS)) ? 31294061 : Item.GetBaseItem().SpriteId);
            this.WriteInteger(Item.GetX);
            this.WriteInteger(Item.GetY);
            this.WriteInteger(Item.Rotation);
            this.WriteString(string.Format("{0:0.00}", Item.GetZ));
            this.WriteString(string.Empty);

            if (Item.Limited > 0)
            {
                this.WriteInteger(1);
                this.WriteInteger(256);
                this.WriteString(Item.ExtraData);
                this.WriteInteger(Item.Limited);
                this.WriteInteger(Item.LimitedStack);
            }
            else
            {
                ItemBehaviourUtility.GenerateExtradata(Item, this);
            }

            this.WriteInteger(-1); // to-do: check
            this.WriteInteger(1); //(Item.GetBaseItem().Modes > 1) ? 1 : 0
            this.WriteInteger(UserId);
        }
    }
}
