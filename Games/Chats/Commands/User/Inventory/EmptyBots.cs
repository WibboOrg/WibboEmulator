namespace WibboEmulator.Games.Chats.Commands.User.Inventory;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Bots;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class EmptyBots : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        session.User.InventoryComponent.ClearBots();
        session.SendPacket(new BotInventoryComposer(session.User.InventoryComponent.GetBots()));
        userRoom.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("empty.cleared", session.Langue));
    }
}
