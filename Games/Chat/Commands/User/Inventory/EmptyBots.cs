namespace WibboEmulator.Games.Chat.Commands.User.Inventory;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Bots;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class EmptyBots : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        session.GetUser().GetInventoryComponent().ClearBots();
        session.SendPacket(new BotInventoryComposer(session.GetUser().GetInventoryComponent().GetBots()));
        session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("empty.cleared", session.Langue));
    }
}
