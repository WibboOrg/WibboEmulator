namespace WibboEmulator.Games.Chats.Commands.Staff.Gestion;
using System.Text;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Loots;
using WibboEmulator.Games.Rooms;

internal sealed class LootboxInfo : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        var basicCount = LootManager.GetRarityCounter(RaretyLevelType.Basic);
        var communCount = LootManager.GetRarityCounter(RaretyLevelType.Commun);
        var epicCount = LootManager.GetRarityCounter(RaretyLevelType.Epic);
        var legendaryCount = LootManager.GetRarityCounter(RaretyLevelType.Legendary);

        var stringBuilder = new StringBuilder()
        .Append("- Information sur le nombre de rare distribué ce mois-ci -\r")
        .Append("Basique: " + basicCount + "\r")
        .Append("Commun: " + communCount + "\r")
        .Append("Epique: " + epicCount + "\r")
        .Append("Legendaire: " + legendaryCount + "\r");

        Session.SendNotification(stringBuilder.ToString());
    }
}
