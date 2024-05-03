namespace WibboEmulator.Games.LandingView;

public class Promotion(int index, string header, string body, string button, bool inGame, string specialAction, string image)
{
    public int Index { get; private set; } = index;
    public string Header { get; private set; } = header;
    public string Body { get; private set; } = body;
    public string Button { get; private set; } = button;
    public bool InGamePromo { get; private set; } = inGame;
    public string SpecialAction { get; private set; } = specialAction;
    public string Image { get; private set; } = image;
}
