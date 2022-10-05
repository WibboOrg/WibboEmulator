namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Items.Wired;

internal class ObjectUpdateComposer : ServerPacket
{
    public ObjectUpdateComposer(Item item, int userId, bool hideWired = false)
        : base(ServerPacketHeader.FURNITURE_FLOOR_UPDATE)
    {
        this.WriteInteger(item.Id);
        this.WriteInteger((hideWired && WiredUtillity.TypeIsWired(item.GetBaseItem().InteractionType) && item.GetBaseItem().InteractionType != InteractionType.HIGHSCORE && item.GetBaseItem().InteractionType != InteractionType.HIGHSCOREPOINTS) ? 31294061 : item.GetBaseItem().SpriteId);
        this.WriteInteger(item.X);
        this.WriteInteger(item.Y);
        this.WriteInteger(item.Rotation);
        this.WriteString(string.Format("{0:0.00}", item.Z));
        this.WriteString(string.Empty);

        ItemBehaviourUtility.GenerateExtradata(item, this);

        this.WriteInteger(-1); // to-do: check
        this.WriteInteger(1); //(Item.GetBaseItem().Modes > 1) ? 1 : 0
        this.WriteInteger(userId);
    }
}
