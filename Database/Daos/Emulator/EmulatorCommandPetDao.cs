namespace WibboEmulator.Database.Daos.Emulator;
using System.Data;
using Dapper;

internal sealed class EmulatorCommandPetDao
{
    internal static List<EmulatorCommandPetEntity> GetAll(IDbConnection dbClient) => dbClient.Query<EmulatorCommandPetEntity>(
        "SELECT `id`, `command` FROM `emulator_command_pet`"
    ).ToList();
}

public class EmulatorCommandPetEntity
{
    public int Id { get; set; }
    public string Command { get; set; }
}
