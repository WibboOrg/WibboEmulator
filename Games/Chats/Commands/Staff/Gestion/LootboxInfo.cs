namespace WibboEmulator.Games.Chats.Commands.Staff.Gestion;
using System.Text;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class LootboxInfo : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var basicCount = WibboEnvironment.GetGame().GetLootManager().GetRarityCounter(1);
        var communCount = WibboEnvironment.GetGame().GetLootManager().GetRarityCounter(2);
        var epicCount = WibboEnvironment.GetGame().GetLootManager().GetRarityCounter(3);
        var legendaryCount = WibboEnvironment.GetGame().GetLootManager().GetRarityCounter(4);

        var stringBuilder = new StringBuilder()
        .Append("- Information sur le nombre de rare distribué ce mois-ci -\r")
        .Append("Basique: " + basicCount + "\r")
        .Append("Commun: " + communCount + "\r")
        .Append("Epique: " + epicCount + "\r")
        .Append("Legendaire: " + legendaryCount + "\r");

        session.SendNotification(stringBuilder.ToString());
    }
}
