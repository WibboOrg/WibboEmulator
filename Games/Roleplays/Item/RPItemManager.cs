namespace WibboEmulator.Games.Roleplays.Item;
using System.Data;
using WibboEmulator.Database.Daos.Roleplay;

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

    public void Init(IDbConnection dbClient)
    {
        this._items.Clear();

        var rpItemList = RoleplayItemDao.GetAll(dbClient);
        if (rpItemList.Count != 0)
        {
            foreach (var rpItem in rpItemList)
            {
                if (!this._items.ContainsKey(rpItem.Id))
                {
                    this._items.Add(rpItem.Id,
                        new RPItem(rpItem.Id,
                        rpItem.Name,
                        rpItem.Desc,
                        rpItem.Price,
                        rpItem.Type,
                        rpItem.Value,
                        rpItem.AllowStack,
                        RPItemCategorys.GetTypeFromString(rpItem.Category)));
                }
            }
        }
    }
}
