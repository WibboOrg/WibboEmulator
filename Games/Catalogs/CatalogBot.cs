namespace WibboEmulator.Games.Catalogs;

public class CatalogBot(int id, string name, string figure, string motto, string gender, string aiType)
{
    public int Id { get; private set; } = id;
    public string Figure { get; private set; } = figure;
    public string Gender { get; private set; } = gender;
    public string Motto { get; private set; } = motto;
    public string Name { get; private set; } = name;
    public string AIType { get; private set; } = aiType;
}
