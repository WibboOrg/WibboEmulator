using System.Text;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class LootboxInfo : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            int basicCount = WibboEnvironment.GetGame().GetLootManager().GetRarityCounter(1);
            int communCount = WibboEnvironment.GetGame().GetLootManager().GetRarityCounter(2);
            int epicCount = WibboEnvironment.GetGame().GetLootManager().GetRarityCounter(3);
            int legendaryCount = WibboEnvironment.GetGame().GetLootManager().GetRarityCounter(4);

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("- Information sur le nombre de rare distribuer ce mois-ci -\r");
            stringBuilder.Append("Basique: " + basicCount + "\r");
            stringBuilder.Append("Commun: " + communCount + "\r");
            stringBuilder.Append("Epique: " + epicCount + "\r");
            stringBuilder.Append("Legendaire: " + legendaryCount + "\r");

            Session.SendNotification(stringBuilder.ToString());
        }
    }
}
