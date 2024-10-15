namespace WibboEmulator.Games.Chats.Commands.User.Inventory;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Bots;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class EmptyBots : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        Session.User.InventoryComponent.ClearBots();
        Session.SendPacket(new BotInventoryComposer(Session.User.InventoryComponent.Bots));
        userRoom.SendWhisperChat(LanguageManager.TryGetValue("empty.cleared", Session.Language));
    }
}
