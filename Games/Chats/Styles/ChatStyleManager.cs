namespace WibboEmulator.Games.Chats.Styles;
using System.Data;
using WibboEmulator.Database.Daos.Emulator;

public sealed class ChatStyleManager
{
    private readonly Dictionary<int, ChatStyle> _styles;

    public ChatStyleManager() => this._styles = new Dictionary<int, ChatStyle>();

    public void Init(IDbConnection dbClient)
    {
        if (this._styles.Count > 0)
        {
            this._styles.Clear();
        }

        var emulatorChatStyleList = EmulatorChatStyleDao.GetAll(dbClient);

        if (emulatorChatStyleList.Count != 0)
        {
            foreach (var emulatorChatStyle in emulatorChatStyleList)
            {
                if (!this._styles.ContainsKey(emulatorChatStyle.Id))
                {
                    this._styles.Add(emulatorChatStyle.Id, new ChatStyle(emulatorChatStyle.Id, emulatorChatStyle.Name, emulatorChatStyle.RequiredRight));
                }
            }
        }
    }

    public bool TryGetStyle(int id, out ChatStyle style) => this._styles.TryGetValue(id, out style);
}
