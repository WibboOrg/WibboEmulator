namespace WibboEmulator.Games.LandingView;

public class Promotion
{
    public int Index { get; private set; }
    public string Header { get; private set; }
    public string Body { get; private set; }
    public string Button { get; private set; }
    public int InGamePromo { get; private set; }
    public string SpecialAction { get; private set; }
    public string Image { get; private set; }

    public Promotion(int index, string header, string body, string button, int inGame, string specialAction, string image)
    {
        this.Index = index;
        this.Header = header;
        this.Body = body;
        this.Button = button;
        this.InGamePromo = inGame;
        this.SpecialAction = specialAction;
        this.Image = image;
    }
}
