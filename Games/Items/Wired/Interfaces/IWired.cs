using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Games.Items.Wired.Interfaces
{
    public interface IWired
    {
        void Dispose();

        void SaveToDatabase(IQueryAdapter dbClient);

        void LoadFromDatabase(DataRow row);

        void OnTrigger(GameClient Session);

        void Init(List<int> intParams, string stringParam, List<int> stuffIds, int selectionCode, int delay, bool isStaff, bool isGod);

        void LoadItems(bool inDatabase = false);
    }
}
