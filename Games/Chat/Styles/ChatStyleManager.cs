namespace WibboEmulator.Games.Chat.Styles;
using System.Data;
using WibboEmulator.Database.Daos.Emulator;
using WibboEmulator.Database.Interfaces;

public sealed class ChatStyleManager
{
    private readonly Dictionary<int, ChatStyle> _styles;

    public ChatStyleManager() => this._styles = new Dictionary<int, ChatStyle>();

    public void Init(IQueryAdapter dbClient)
    {
        if (this._styles.Count > 0)
        {
            this._styles.Clear();
        }

        var table = EmulatorChatStyleDao.GetAll(dbClient);

        if (table != null)
        {
            foreach (DataRow row in table.Rows)
            {
                if (!this._styles.ContainsKey(Convert.ToInt32(row["id"])))
                {
                    this._styles.Add(Convert.ToInt32(row["id"]), new ChatStyle(Convert.ToInt32(row["id"]), Convert.ToString(row["name"]), Convert.ToString(row["required_right"])));
                }
            }
        }
    }

    public bool TryGetStyle(int id, out ChatStyle style) => this._styles.TryGetValue(id, out style);
}
