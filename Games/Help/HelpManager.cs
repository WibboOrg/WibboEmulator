namespace WibboEmulator.Games.Help;

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

        var list = new List<int>();

        foreach (var entry in this.GuidesOnDuty)
        {
            if (entry.Value)
            {
                continue;
            }

            list.Add(entry.Key);
        }

        if (list.Count == 0)
        {
            return 0;
        }

        var randomId = list[WibboEnvironment.GetRandomNumber(0, list.Count - 1)];
        this.GuidesOnDuty[randomId] = true;

        return randomId;
    }

    public void EndService(int id)
    {
        if (!this.GuidesOnDuty.ContainsKey(id))
        {
            return;
        }

        this.GuidesOnDuty[id] = false;
    }

    public void AddGuide(int guide)
    {
        if (this.GuidesOnDuty.ContainsKey(guide))
        {
            return;
        }

        this.GuidesOnDuty.Add(guide, false);
    }

    public void RemoveGuide(int guide)
    {
        if (!this.GuidesOnDuty.ContainsKey(guide))
        {
            return;
        }

        _ = this.GuidesOnDuty.Remove(guide);
    }
}
