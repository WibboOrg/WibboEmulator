namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Bots;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class EmptyBots : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        session.GetUser().GetInventoryComponent().ClearBots();
        session.SendPacket(new BotInventoryComposer(session.GetUser().GetInventoryComponent().GetBots()));
        session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("empty.cleared", session.Langue));
    }
}
