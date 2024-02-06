namespace WibboEmulator.Games.Helps;

public class HelpManager
{
    public Dictionary<int, bool> GuidesOnDuty { get; set; } = new();

    public int Count => this.GuidesOnDuty.Count;

    public int RandomAvailableGuide()
    {
        if (this.Count == 0)
        {
            return 0;
        }

        var availableGuideIds = this.SelectAvailableGuides();

        return availableGuideIds[WibboEnvironment.GetRandomNumber(0, availableGuideIds.Length - 1)];
    }

    public void GuideLeftService(int id) => this.MarkAsAvailable(id);

    private void MarkAsAvailable(int id)
    {
        if (!this.GuidesOnDuty.TryGetValue(id, out var onDuty))
        {
            return;
        }

        if (onDuty)
        {
            return;
        }

        this.GuidesOnDuty[id] = true;
    }

    private int[] SelectAvailableGuides() => this.GuidesOnDuty.Where(g => !g.Value).Select(g => g.Key).ToArray();

    public void MarkAsOffDuty(int id)
    {
        if (!this.GuidesOnDuty.TryGetValue(id, out var onDuty))
        {
            return;
        }

        if (!onDuty)
        {
            return;
        }

        this.GuidesOnDuty[id] = false;
    }

    public bool TryAddGuide(int guide)
    {
        if (!this.GuidesOnDuty.ContainsKey(guide))
        {
            this.GuidesOnDuty.Add(guide, false);
            return true;
        }

        return false;
    }

    public void TryRemoveGuide(int guide) => _ = this.GuidesOnDuty.Remove(guide);
}
