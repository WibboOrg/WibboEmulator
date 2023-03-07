namespace WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.GameClients;

public interface IWired
{
    void Dispose();

    void SaveToDatabase(IQueryAdapter dbClient);

    void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay);

    void OnTrigger(GameClient session);

    void Init(List<int> intParams, string stringParam, List<int> stuffIds, int selectionCode, int delay, bool isStaff, bool isGod);

    void LoadItems(bool inDatabase = false);
}
