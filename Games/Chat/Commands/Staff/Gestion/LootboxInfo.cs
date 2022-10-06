namespace WibboEmulator.Games.Chat.Commands.Staff.Gestion;
using System.Text;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class LootboxInfo : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        var basicCount = WibboEnvironment.GetGame().GetLootManager().GetRarityCounter(1);
        var communCount = WibboEnvironment.GetGame().GetLootManager().GetRarityCounter(2);
        var epicCount = WibboEnvironment.GetGame().GetLootManager().GetRarityCounter(3);
        var legendaryCount = WibboEnvironment.GetGame().GetLootManager().GetRarityCounter(4);

        var stringBuilder = new StringBuilder();

        stringBuilder.Append("- Information sur le nombre de rare distribuer ce mois-ci -\r");
        stringBuilder.Append("Basique: " + basicCount + "\r");
        stringBuilder.Append("Commun: " + communCount + "\r");
        stringBuilder.Append("Epique: " + epicCount + "\r");
        stringBuilder.Append("Legendaire: " + legendaryCount + "\r");

        session.SendNotification(stringBuilder.ToString());
    }
}
