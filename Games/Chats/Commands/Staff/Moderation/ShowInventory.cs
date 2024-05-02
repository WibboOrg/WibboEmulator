namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;

using WibboEmulator.Core.Language;
using WibboEmulator.Core.Settings;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class ShowInventory : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var targetUser = GameClientManager.GetClientByUsername(parameters[1]);
        if (targetUser == null || targetUser.User == null || targetUser.User.InventoryComponent == null)
        {
            userRoom.SendWhisperChat(LanguageManager.TryGetValue("input.usernotfound", session.Language));
            return;
        }

        targetUser.User.InventoryComponent.LoadInventory();

        var itemRareCount = new Dictionary<string, int>();
        foreach (var item in targetUser.User.InventoryComponent.GetWallAndFloor)
        {
            if (item.ItemData.IsRare == false)
            {
                continue;
            }

            if (itemRareCount.TryGetValue(item.ItemData.ItemName, out _))
            {
                itemRareCount[item.ItemData.ItemName]++;
            }
            else
            {
                itemRareCount.Add(item.ItemData.ItemName, 1);
            }
        }

        var assetsUrl = SettingsManager.GetData<string>("assets.url");

        var output = "Inventaire rare de " + targetUser.User.Username + ":<br><br>";
        output += "<div class=\"overflow-auto grid gap-2\" style=\"--bs-columns: 5; --nitro-grid-column-min-height: 40px; grid-template-columns: repeat(auto-fill, minmax(40px, 1fr));\">";
        foreach (var item in itemRareCount.Take(50))
        {
            output += $"<div class=\"d-flex overflow-hidden position-relative flex-column gap-2 align-items-center justify-content-center layout-grid-item border border-2 border-muted rounded\"><img src=\"//{assetsUrl}/icons/{item.Key}_icon.png\"><div class=\"position-absolute badge border border-black bg-danger px-1 nitro-item-count\">{item.Value}</div></div>";
        }
        output += "</div>";

        session.SendNotification(output);
    }
}
