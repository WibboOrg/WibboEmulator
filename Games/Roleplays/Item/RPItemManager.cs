namespace WibboEmulator.Games.Roleplays.Item;
using System.Data;
using WibboEmulator.Database.Daos.Roleplay;
using WibboEmulator.Database.Interfaces;

public class RPItemManager
{
    private readonly Dictionary<int, RPItem> _items;

    public RPItemManager() => this._items = new Dictionary<int, RPItem>();

    public RPItem GetItem(int id)
    {
        if (!this._items.ContainsKey(id))
        {
            return null;
        }

        _ = this._items.TryGetValue(id, out var item);
        return item;
    }

    public void Init(IQueryAdapter dbClient)
    {
        this._items.Clear();
        var table = RoleplayItemDao.GetAll(dbClient);
        if (table != null)
        {
            foreach (DataRow dataRow in table.Rows)
            {
                if (!this._items.ContainsKey(Convert.ToInt32(dataRow["id"])))
                {
                    this._items.Add(Convert.ToInt32(dataRow["id"]),
                        new RPItem(Convert.ToInt32(dataRow["id"]),
                        (string)dataRow["name"],
                        (string)dataRow["desc"],
                        Convert.ToInt32(dataRow["price"]),
                        (string)dataRow["type"],
                        Convert.ToInt32(dataRow["value"]),
                        (string)dataRow["allowstack"] == "1",
                        RPItemCategorys.GetTypeFromString((string)dataRow["category"])));
                }
            }
        }
    }
}
