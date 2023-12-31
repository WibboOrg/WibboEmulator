namespace WibboEmulator.Core.FigureData;
using System.Text.Json;
using WibboEmulator.Core.FigureData.JsonObject;
using WibboEmulator.Core.FigureData.Types;

public class FigureDataManager
{
    private readonly List<string> _requirements;
    private readonly Dictionary<int, Palette> _palettes;
    private readonly Dictionary<string, FigureSet> _setTypes;

    public FigureDataManager()
    {
        this._palettes = new Dictionary<int, Palette>();
        this._setTypes = new Dictionary<string, FigureSet>();

        this._requirements = new List<string>
        {
            "hd",
            "ch",
            "lg"
        };
    }

    public void Init()
    {
        if (this._palettes.Count > 0)
        {
            this._palettes.Clear();
        }

        if (this._setTypes.Count > 0)
        {
            this._setTypes.Clear();
        }

        var figureUrl = WibboEnvironment.GetSettings().GetData<string>("figuredata.url") + "?cache=" + WibboEnvironment.GetUnixTimestamp();

        var response = WibboEnvironment.GetHttpClient().GetAsync(figureUrl).GetAwaiter().GetResult();

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
            this._palettes.Add(palette.Id, new Palette(palette.Id));

            foreach (var color in palette.Colors)
            {
                this._palettes[palette.Id].Colors.Add(Convert.ToInt32(color.Id), new Color(color.Id, color.Index, color.Club, color.Selectable));
            }
        }

        foreach (var child in figureData.SetTypes)
        {
            this._setTypes.Add(child.Type, new FigureSet(SetTypeUtility.GetSetType(child.Type), child.PaletteId));

            foreach (var set in child.Sets)
            {
                if (!this._setTypes[child.Type].Sets.ContainsKey(set.Id))
                {
                    this._setTypes[child.Type].Sets.Add(set.Id, new Set(set.Id, set.Gender, set.Club, set.Colorable));
                }

                foreach (var part in set.Parts)
                {
                    if (part.Type != null)
                    {
                        if (!this._setTypes[child.Type].Sets[set.Id].Parts.ContainsKey(part.Id + "-" + part.Type))
                        {
                            this._setTypes[child.Type].Sets[set.Id].Parts.Add(part.Id + "-" + part.Type,
                          new Part(part.Id, SetTypeUtility.GetSetType(child.Type), part.Colorable, part.Index, part.Colorindex));
                        }
                    }
                }
            }
        }

        //Faceless.
        this._setTypes["hd"].Sets.Add(99999, new Set(99999, "U", 0, true));
    }

    public string ProcessFigure(string figure, string gender, bool hasClub)
    {
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

                if (this._setTypes.TryGetValue(type, out var figureSet))
                {
                    var splitpart = part.Split('-');
                    if (splitpart.Length < 2)
                    {
                        continue;
                    }

                    var partId = Convert.ToInt32(splitpart[1]);
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

                                colorId = this.GetRandomColor(figureSet.PalletId);
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
                                        colorId = Convert.ToInt32(part.Split('-')[2]);

                                        var palette = this.GetPalette(colorId);
                                        if (palette != null && colorId != 0)
                                        {
                                            if (figureSet.PalletId != palette.Id)
                                            {
                                                colorId = this.GetRandomColor(figureSet.PalletId);
                                            }
                                        }
                                        else if (palette == null && colorId != 0)
                                        {
                                            colorId = this.GetRandomColor(figureSet.PalletId);
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
                                        secondColorId = Convert.ToInt32(part.Split('-')[3]);

                                        var palette = this.GetPalette(secondColorId);
                                        if (palette != null && secondColorId != 0)
                                        {
                                            if (figureSet.PalletId != palette.Id)
                                            {
                                                secondColorId = this.GetRandomColor(figureSet.PalletId);
                                            }
                                        }
                                        else if (palette == null && secondColorId != 0)
                                        {
                                            secondColorId = this.GetRandomColor(figureSet.PalletId);
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
                                        colorId = Convert.ToInt32(part.Split('-')[2]);
                                    }
                                }
                            }
                        }

                        if (set.ClubLevel > 0 && !hasClub)
                        {
                            partId = figureSet.Sets.FirstOrDefault(x => x.Value.Gender == gender || (x.Value.Gender == "U" && x.Value.ClubLevel == 0)).Value.Id;

                            _ = figureSet.Sets.TryGetValue(partId, out set);

                            colorId = this.GetRandomColor(figureSet.PalletId);
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

            foreach (var requirement in this._requirements)
            {
                if (!rebuildFigure.Contains(requirement))
                {
                    if (requirement == "ch" && gender == "M")
                    {
                        continue;
                    }

                    if (this._setTypes.TryGetValue(requirement, out var figureSet))
                    {
                        var set = figureSet.Sets.FirstOrDefault(x => x.Value.Gender == gender || x.Value.Gender == "U").Value;
                        if (set != null)
                        {
                            var partId = figureSet.Sets.FirstOrDefault(x => x.Value.Gender == gender || x.Value.Gender == "U").Value.Id;
                            var colorId = this.GetRandomColor(figureSet.PalletId);

                            rebuildFigure = rebuildFigure + requirement + "-" + partId + "-" + colorId + ".";
                        }
                    }
                }
            }

            return rebuildFigure.Remove(rebuildFigure.Length - 1);
        }
        catch { }
        return "hd-180-1.lg-270-1408";
    }

    public Palette GetPalette(int colorId) => this._palettes.FirstOrDefault(x => x.Value.Colors.ContainsKey(colorId)).Value;

    public bool TryGetPalette(int palletId, out Palette palette) => this._palettes.TryGetValue(palletId, out palette);

    public int GetRandomColor(int palletId) => this._palettes[palletId].Colors.FirstOrDefault().Value.Id;
}
