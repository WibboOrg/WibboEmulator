namespace WibboEmulator.Core.FigureData;
using System.Text.Json;
using WibboEmulator.Core.FigureData.JsonObject;
using WibboEmulator.Core.FigureData.Types;
using WibboEmulator.Core.Settings;

public static class FigureDataManager
{
    private static readonly List<string> Requirements = ["hd", "ch", "lg"];
    private static readonly Dictionary<int, Palette> Palettes = [];
    private static readonly Dictionary<string, FigureSet> SetTypes = [];

    public static void Initialize()
    {
        Palettes.Clear();
        SetTypes.Clear();

        var figureUrl = SettingsManager.GetData<string>("figuredata.url") + "?cache=" + WibboEnvironment.GetUnixTimestamp();

        var response = WibboEnvironment.HttpClient.GetAsync(figureUrl).GetAwaiter().GetResult();

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("Error load figuredata");
            return;
        }

        var jsonString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        var figureData = JsonSerializer.Deserialize<FigureDataRoot>(jsonString, options);

        if (figureData == null)
        {
            Console.WriteLine("Error parse figuredata");
            return;
        }

        foreach (var palette in figureData.Palettes)
        {
            Palettes.Add(palette.Id, new Palette(palette.Id));

            foreach (var color in palette.Colors)
            {
                Palettes[palette.Id].Colors.Add(Convert.ToInt32(color.Id), new Color(color.Id, color.Index, color.Club, color.Selectable));
            }
        }

        foreach (var child in figureData.SetTypes)
        {
            SetTypes.Add(child.Type, new FigureSet(SetTypeUtility.GetSetType(child.Type), child.PaletteId));

            foreach (var set in child.Sets)
            {
                if (!SetTypes[child.Type].Sets.TryGetValue(set.Id, out var sets))
                {
                    SetTypes[child.Type].Sets.Add(set.Id, new Set(set.Id, set.Gender, set.Club, set.Colorable));
                }

                foreach (var part in set.Parts)
                {
                    if (part.Type != null)
                    {
                        if (sets != null && !sets.Parts.ContainsKey(part.Id + "-" + part.Type))
                        {
                            SetTypes[child.Type].Sets[set.Id].Parts.Add(part.Id + "-" + part.Type,
                               new Part(part.Id, SetTypeUtility.GetSetType(child.Type), part.Colorable, part.Index, part.Colorindex));
                        }
                    }
                }
            }
        }

        //Faceless.
        SetTypes["hd"].Sets.Add(99999, new Set(99999, "U", 0, true));
    }

    public static string ProcessFigure(string figure, string gender, bool hasClub)
    {
        if (figure == string.Empty || gender == string.Empty)
        {
            return "hd-180-1.lg-270-1408";
        }

        try
        {
            figure = figure.ToLower();
            gender = gender.ToUpper();

            var rebuildFigure = string.Empty;

            var figureParts = figure.Split('.');
            foreach (var part in figureParts.ToList())
            {
                if (!part.Contains('-'))
                {
                    continue;
                }

                var type = part.Split('-')[0];

                if (SetTypes.TryGetValue(type, out var figureSet))
                {
                    var splitpart = part.Split('-');
                    if (splitpart.Length < 2)
                    {
                        continue;
                    }

                    if (!int.TryParse(splitpart[1], out var partId))
                    {
                        continue;
                    }

                    var colorId = 0;
                    var secondColorId = 0;

                    if (figureSet.Sets.TryGetValue(partId, out var set))
                    {
                        if (set.Gender != gender && set.Gender != "U")
                        {
                            if (figureSet.Sets.Any(x => x.Value.Gender == gender || x.Value.Gender == "U"))
                            {
                                partId = figureSet.Sets.FirstOrDefault(x => x.Value.Gender == gender || x.Value.Gender == "U").Value.Id;

                                //Fetch the new set.
                                _ = figureSet.Sets.TryGetValue(partId, out set);

                                colorId = GetRandomColor(figureSet.PalletId);
                            }
                            else
                            {
                                //No replacable?
                            }
                        }

                        if (set == null)
                        {
                            continue;
                        }

                        if (set.Colorable)
                        {
                            //Couldn't think of a better way to split the colors, if I looped the parts I still have to remove Type-PartId, then loop color 1 & color 2. Meh

                            var splitterCounter = part.Count(x => x == '-');
                            if (splitterCounter is 2 or 3)
                            {
                                if (!string.IsNullOrEmpty(part.Split('-')[2]))
                                {
                                    if (int.TryParse(part.Split('-')[2], out colorId))
                                    {
                                        var palette = GetPalette(colorId);
                                        if (palette != null && colorId != 0)
                                        {
                                            if (figureSet.PalletId != palette.Id)
                                            {
                                                colorId = GetRandomColor(figureSet.PalletId);
                                            }
                                        }
                                        else if (palette == null && colorId != 0)
                                        {
                                            colorId = GetRandomColor(figureSet.PalletId);
                                        }
                                    }
                                    else
                                    {
                                        colorId = 0;
                                    }
                                }
                                else
                                {
                                    colorId = 0;
                                }
                            }

                            if (splitterCounter == 3)
                            {
                                if (!string.IsNullOrEmpty(part.Split('-')[3]))
                                {
                                    if (int.TryParse(part.Split('-')[3], out secondColorId))
                                    {
                                        var palette = GetPalette(secondColorId);
                                        if (palette != null && secondColorId != 0)
                                        {
                                            if (figureSet.PalletId != palette.Id)
                                            {
                                                secondColorId = GetRandomColor(figureSet.PalletId);
                                            }
                                        }
                                        else if (palette == null && secondColorId != 0)
                                        {
                                            secondColorId = GetRandomColor(figureSet.PalletId);
                                        }
                                    }
                                    else
                                    {
                                        secondColorId = 0;
                                    }
                                }
                                else
                                {
                                    secondColorId = 0;
                                }
                            }
                        }
                        else
                        {
                            var ignore = new string[] { "ca", "wa" };

                            if (ignore.Contains(type))
                            {
                                var splitterCounter = part.Count(x => x == '-');
                                if (splitterCounter > 1)
                                {
                                    if (!string.IsNullOrEmpty(part.Split('-')[2]))
                                    {
                                        _ = int.TryParse(part.Split('-')[2], out colorId);
                                    }
                                }
                            }
                        }

                        if (set.ClubLevel > 0 && !hasClub)
                        {
                            partId = figureSet.Sets.FirstOrDefault(x => x.Value.Gender == gender || (x.Value.Gender == "U" && x.Value.ClubLevel == 0)).Value.Id;

                            _ = figureSet.Sets.TryGetValue(partId, out set);

                            colorId = GetRandomColor(figureSet.PalletId);
                        }

                        if (secondColorId == 0)
                        {
                            rebuildFigure = rebuildFigure + type + "-" + partId + "-" + colorId + ".";
                        }
                        else
                        {
                            rebuildFigure = rebuildFigure + type + "-" + partId + "-" + colorId + "-" + secondColorId + ".";
                        }
                    }
                }
            }

            foreach (var requirement in Requirements)
            {
                if (!rebuildFigure.Contains(requirement))
                {
                    if (requirement == "ch" && gender == "M")
                    {
                        continue;
                    }

                    if (SetTypes.TryGetValue(requirement, out var figureSet))
                    {
                        var set = figureSet.Sets.FirstOrDefault(x => x.Value.Gender == gender || x.Value.Gender == "U").Value;
                        if (set != null)
                        {
                            var partId = figureSet.Sets.FirstOrDefault(x => x.Value.Gender == gender || x.Value.Gender == "U").Value.Id;
                            var colorId = GetRandomColor(figureSet.PalletId);

                            rebuildFigure = rebuildFigure + requirement + "-" + partId + "-" + colorId + ".";
                        }
                    }
                }
            }

            return rebuildFigure.Remove(rebuildFigure.Length - 1);
        }
        catch (Exception ex)
        {
            ExceptionLogger.LogException(ex.ToString());
        }

        return "hd-180-1.lg-270-1408";
    }

    public static Palette GetPalette(int colorId) => Palettes.FirstOrDefault(x => x.Value.Colors.ContainsKey(colorId)).Value;

    public static bool TryGetPalette(int palletId, out Palette palette) => Palettes.TryGetValue(palletId, out palette);

    public static int GetRandomColor(int palletId) => Palettes[palletId].Colors.FirstOrDefault().Value.Id;
}
