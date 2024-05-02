namespace WibboEmulator.Games.Chats.Styles;
using System.Data;
using WibboEmulator.Database.Daos.Emulator;

public static class ChatStyleManager
{
    private static readonly Dictionary<int, ChatStyle> Styles = new();

    public static void Initialize(IDbConnection dbClient)
    {
        Styles.Clear();

        var emulatorChatStyleList = EmulatorChatStyleDao.GetAll(dbClient);

        foreach (var emulatorChatStyle in emulatorChatStyleList)
        {
            if (!Styles.ContainsKey(emulatorChatStyle.Id))
            {
                Styles.Add(emulatorChatStyle.Id, new ChatStyle(emulatorChatStyle.Id, emulatorChatStyle.Name, emulatorChatStyle.RequiredRight));
            }
        }
    }

    public static bool TryGetStyle(int id, out ChatStyle style) => Styles.TryGetValue(id, out style);
}
