namespace WibboEmulator.Games.Helps;

using WibboEmulator.Utilities;

public static class HelpManager
{
    public static Dictionary<int, bool> GuidesOnDuty { get; set; } = new();

    public static int Count => GuidesOnDuty.Count;

    public static int RandomAvailableGuide()
    {
        if (Count == 0)
        {
            return 0;
        }

        var availableGuideIds = SelectAvailableGuides();

        return availableGuideIds.GetRandomElement();
    }

    public static void GuideLeftService(int id) => MarkAsAvailable(id);

    private static void MarkAsAvailable(int id)
    {
        if (!GuidesOnDuty.TryGetValue(id, out var onDuty))
        {
            return;
        }

        if (onDuty)
        {
            return;
        }

        GuidesOnDuty[id] = true;
    }

    private static int[] SelectAvailableGuides() => GuidesOnDuty.Where(g => !g.Value).Select(g => g.Key).ToArray();

    public static void MarkAsOffDuty(int id)
    {
        if (!GuidesOnDuty.TryGetValue(id, out var onDuty))
        {
            return;
        }

        if (!onDuty)
        {
            return;
        }

        GuidesOnDuty[id] = false;
    }

    public static bool TryAddGuide(int guide)
    {
        if (!GuidesOnDuty.ContainsKey(guide))
        {
            GuidesOnDuty.Add(guide, false);
            return true;
        }

        return false;
    }

    public static void TryRemoveGuide(int guide) => _ = GuidesOnDuty.Remove(guide);
}
