namespace WibboEmulator.Games.Items.Wired.Interfaces;
using System.Data;
using WibboEmulator.Games.GameClients;

public interface IWired
{
    void Dispose();

    void SaveToDatabase(IDbConnection dbClient);

    void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay);

    void OnTrigger(GameClient Session);

    void Initialize(List<int> intParams, string stringParam, List<int> stuffIds, int selectionCode, int delay, bool isStaff, bool isGod);

    void LoadItems(bool inDatabase = false);
}
