using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using System.Data;

namespace WibboEmulator.Games.Chat.Styles
{
    public sealed class ChatStyleManager
    {
        private readonly Dictionary<int, ChatStyle> _styles;

        public ChatStyleManager()
        {
            this._styles = new Dictionary<int, ChatStyle>();
        }

        public void Init(IQueryAdapter dbClient)
        {
            if (this._styles.Count > 0)
            {
                this._styles.Clear();
            }

            DataTable Table = EmulatorChatStyleDao.GetAll(dbClient);

            if (Table != null)
            {
                foreach (DataRow Row in Table.Rows)
                {
                    if (!this._styles.ContainsKey(Convert.ToInt32(Row["id"])))
                    {
                        this._styles.Add(Convert.ToInt32(Row["id"]), new ChatStyle(Convert.ToInt32(Row["id"]), Convert.ToString(Row["name"]), Convert.ToString(Row["required_right"])));
                    }
                }
            }
        }

        public bool TryGetStyle(int Id, out ChatStyle Style)
        {
            return this._styles.TryGetValue(Id, out Style);
        }
    }
}
