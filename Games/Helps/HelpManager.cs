namespace WibboEmulator.Games.Helps;

public class HelpManager
{
    public Dictionary<int, bool> GuidesOnDuty { get; set; } = new();

    public int GuidesCount => this.GuidesOnDuty.Count;

    public int GetRandomGuide()
    {
        if (this.GuidesCount == 0)
        {
            return 0;
        }

        var availableGuideIds = this.GuidesOnDuty
            .Where(guide => !guide.Value)
            .Select(guide => guide.Key)
            .ToList();

        if (availableGuideIds.Count == 0)
        {
            return 0;
        }

        var randomId = availableGuideIds[WibboEnvironment.GetRandomNumber(0, availableGuideIds.Count - 1)];
        this.GuidesOnDuty[randomId] = true;

        return randomId;
    }

    public void EndService(int id)
    {
        if (this.GuidesOnDuty.ContainsKey(id))
        {
            this.GuidesOnDuty[id] = false;
        }
    }

    public void AddGuide(int guide)
    {
        if (!this.GuidesOnDuty.ContainsKey(guide))
        {
            this.GuidesOnDuty.Add(guide, false);
        }
    }

    public void RemoveGuide(int guide) => _ = this.GuidesOnDuty.Remove(guide);
}
