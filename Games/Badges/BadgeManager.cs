namespace WibboEmulator.Games.Badges;

public class BadgeManager
{
    private readonly List<string> _disallowedBadgePrefixes;
    private readonly List<string> _notAllowed;

    public BadgeManager()
    {
        this._disallowedBadgePrefixes = new List<string>
        {
            "MRUN",
            "WORLDRUNSAVE",
            "ACH_"
        };

        this._notAllowed = new List<string>();
    }

    public void Initialize()
    {
        this._notAllowed.Clear();
        this.AddDisallowedFromConfig();
    }

    private void AddDisallowedFromConfig(string configString = "badge.not.allowed")
    {
        var badgeNotAllowed = WibboEnvironment.GetSettings().GetData<string>(configString);
        this._notAllowed.AddRange(badgeNotAllowed?.Split(',') ?? Array.Empty<string>());
    }

    public bool HaveNotAllowed(string badgeId) => this._notAllowed.Contains(badgeId) || this.IsDisallowedByType(badgeId);

    private bool IsDisallowedByType(string badgeId) => this._disallowedBadgePrefixes.Exists(badgeId.StartsWith);
}