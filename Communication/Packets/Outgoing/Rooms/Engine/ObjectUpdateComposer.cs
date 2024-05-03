namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;

using WibboEmulator.Core.Settings;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Items.Wired;

internal sealed class ObjectUpdateComposer : ServerPacket
{
    public ObjectUpdateComposer(Item item, int userId, bool hideWired = false)
        : base(ServerPacketHeader.FURNITURE_FLOOR_UPDATE)
    {
        this.WriteInteger(item.Id);
        this.WriteInteger(hideWired && WiredUtillity.AllowHideWiredType(item.ItemData.InteractionType) ? SettingsManager.GetData<int>("wired.hide.item.id") : item.ItemData.SpriteId);
        this.WriteInteger(item.X);
        this.WriteInteger(item.Y);
        this.WriteInteger(item.Rotation);
        this.WriteString(string.Format(/*lang=json*/ "{0:0.00}", item.Z));
        this.WriteString(item.Data.Height.ToString());
        this.WriteInteger(item.Extra);

        ItemBehaviourUtility.GenerateExtradata(item, this);

        this.WriteInteger(-1); // to-do: check
        this.WriteInteger(1); //(Item.GetBaseItem().Modes > 1) ? 1 : 0
        this.WriteInteger(userId);
    }
}
