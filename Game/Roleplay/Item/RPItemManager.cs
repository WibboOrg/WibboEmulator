using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Roleplay.Item;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Roleplay
{
    public class RPItemManager
    {
        private readonly Dictionary<int, RPItem> _items;

        public RPItemManager()
        {
            this._items = new Dictionary<int, RPItem>();
        }

        public RPItem GetItem(int Id)
        {
            if (!this._items.ContainsKey(Id))
            {
                return null;
            }

            this._items.TryGetValue(Id, out RPItem item);
            return item;
        }

        public void Init()
        {
            this._items.Clear();
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DataTable table = RoleplayItemDao.GetAll(dbClient);
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
                                ((string)dataRow["allowstack"]) == "1",
                                RPItemCategorys.GetTypeFromString((string)dataRow["category"])));
                        }
                    }
                }
            }
        }
    }
}
