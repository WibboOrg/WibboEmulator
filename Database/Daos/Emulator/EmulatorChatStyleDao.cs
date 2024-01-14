namespace WibboEmulator.Database.Daos.Emulator;
using System.Data;
using Dapper;

internal sealed class EmulatorChatStyleDao
{
    internal static List<EmulatorChatStyleEntity> GetAll(IDbConnection dbClient) => dbClient.Query<EmulatorChatStyleEntity>(
        "SELECT id, name, required_right FROM `emulator_chat_style`"
    ).ToList();
}

public class EmulatorChatStyleEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string RequiredRight { get; set; }
}
