namespace WibboEmulator.Games.Roleplays.Item;
using System.Data;
using WibboEmulator.Database.Daos.Roleplay;

public static class RPItemManager
{
    private static readonly Dictionary<int, RPItem> Items = [];

    public static RPItem GetItem(int id)
    {
        if (!Items.ContainsKey(id))
        {
            return null;
        }

        _ = Items.TryGetValue(id, out var item);
        return item;
    }

    public static void Initialize(IDbConnection dbClient)
    {
        Items.Clear();

        var rpItemList = RoleplayItemDao.GetAll(dbClient);
        if (rpItemList.Count != 0)
        {
            foreach (var rpItem in rpItemList)
            {
                if (!Items.ContainsKey(rpItem.Id))
                {
                    Items.Add(rpItem.Id,
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
