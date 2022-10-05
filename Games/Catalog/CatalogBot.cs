namespace WibboEmulator.Games.Catalog;

public class CatalogBot
{
    public int Id { get; private set; }
    public string Figure { get; private set; }
    public string Gender { get; private set; }
    public string Motto { get; private set; }
    public string Name { get; private set; }
    public string AIType { get; private set; }

    public CatalogBot(int id, string name, string figure, string motto, string gender, string aiType)
    {
        this.Id = id;
        this.Name = name;
        this.Figure = figure;
        this.Motto = motto;
        this.Gender = gender;
        this.AIType = aiType;
    }
}
