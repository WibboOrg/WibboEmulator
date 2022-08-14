using WibboEmulator.Game.Items;
using WibboEmulator.Game.Items.Wired;

namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class ObjectUpdateComposer : ServerPacket
    {
        public ObjectUpdateComposer(Item Item, int UserId, bool HideWired = false)
            : base(ServerPacketHeader.FURNITURE_FLOOR_UPDATE)
        {
            this.WriteInteger(Item.Id);
            this.WriteInteger((HideWired && WiredUtillity.TypeIsWired(Item.GetBaseItem().InteractionType) && (Item.GetBaseItem().InteractionType != InteractionType.HIGHSCORE && Item.GetBaseItem().InteractionType != InteractionType.HIGHSCOREPOINTS)) ? 31294061 : Item.GetBaseItem().SpriteId);
            this.WriteInteger(Item.X);
            this.WriteInteger(Item.Y);
            this.WriteInteger(Item.Rotation);
            this.WriteString(string.Format("{0:0.00}", Item.Z));
            this.WriteString(string.Empty);

            ItemBehaviourUtility.GenerateExtradata(Item, this);

            this.WriteInteger(-1); // to-do: check
            this.WriteInteger(1); //(Item.GetBaseItem().Modes > 1) ? 1 : 0
            this.WriteInteger(UserId);
        }
    }
}
