namespace WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Wireds;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

public class WiredTriggerBase : WiredBase
{
    internal WiredTriggerBase(Item item, Room room, int type) : base(item, room, type)
    {

    }

    public override void OnTrigger(GameClient session) => session.SendPacket(new WiredFurniTriggerComposer(this.StuffTypeSelectionEnabled, this.FurniLimit, this.StuffIds, this.StuffTypeId, this.Id,
            this.StringParam, this.IntParams, this.StuffTypeSelectionCode, this.Type, this.Conflicting));
}
