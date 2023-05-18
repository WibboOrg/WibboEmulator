namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;

using WibboEmulator.Games.Catalogs.Utilities;
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

        var targetUser = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(parameters[1]);
        if (targetUser == null || targetUser.User == null || targetUser.User.InventoryComponent == null)
        {
            userRoom.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", session.Langue));
            return;
        }

        targetUser.User.InventoryComponent.LoadInventory();

        var itemRareCount = new Dictionary<string, int>();
        foreach (var item in targetUser.User.InventoryComponent.GetWallAndFloor)
        {
            if (item.GetBaseItem().IsRare == false)
            {
                continue;
            }

            if (itemRareCount.TryGetValue(item.GetBaseItem().ItemName, out _))
            {
                itemRareCount[item.GetBaseItem().ItemName]++;
            }
            else
            {
                itemRareCount.Add(item.GetBaseItem().ItemName, 1);
            }
        }

        var output = "Inventaire rare de " + targetUser.User.Username + ":<br><br>";
        output += "<div class=\"overflow-auto grid gap-2\" style=\"--bs-columns: 5; --nitro-grid-column-min-height: 40px; grid-template-columns: repeat(auto-fill, minmax(40px, 1fr));\">";
        foreach (var item in itemRareCount)
        {
            output += $"<div class=\"d-flex overflow-hidden position-relative cursor-pointer flex-column gap-2 align-items-center justify-content-center layout-grid-item border border-2 border-muted rounded\"><img src=\"https://assets.wibbo.org/icons/{item.Key}_icon.png\"><div class=\"position-absolute badge border border-black bg-danger px-1 nitro-item-count\">{item.Value}</div></div>";
        }
        output += "</div>";

        session.SendNotification(output);
    }
}
