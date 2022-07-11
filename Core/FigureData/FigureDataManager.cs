using WibboEmulator.Core.FigureData.Types;
using WibboEmulator.Core.FigureData.JsonObject;
using System.Xml;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WibboEmulator.Core.FigureData
{
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

        public async void Init()
        {
            if (this._palettes.Count > 0)
            {
                this._palettes.Clear();
            }

            if (this._setTypes.Count > 0)
            {
                this._setTypes.Clear();
            }

            HttpResponseMessage response = await WibboEnvironment.GetHttpClient().GetAsync(WibboEnvironment.FigureDataUrl + "?cache=" + WibboEnvironment.GetUnixTimestamp());

            if(!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Error load figuredata");
                return;
            }

            string jsonString = await response.Content.ReadAsStringAsync();

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            FigureDataRoot? figureData = JsonSerializer.Deserialize<FigureDataRoot>(jsonString, options);

            foreach (FigureDataPalette palette in figureData.Palettes)
            {
                this._palettes.Add(palette.Id, new Palette(palette.Id));

                foreach (FigureDataColor color in palette.Colors)
                {
                    this._palettes[palette.Id].Colors.Add(Convert.ToInt32(color.Id), new Color(color.Id, color.Index, color.Club, color.Selectable));
                }
            }

            foreach (FigureDataSetType Child in figureData.SetTypes)
            {
                this._setTypes.Add(Child.Type, new FigureSet(SetTypeUtility.GetSetType(Child.Type), Child.PaletteId));

                foreach (FigureDataSet set in Child.Sets)
                {
                    if (!this._setTypes[Child.Type].Sets.ContainsKey(set.Id))
                        this._setTypes[Child.Type].Sets.Add(set.Id, new Set(set.Id, set.Gender, set.Club, set.Colorable));

                    foreach (FigureDataPart part in set.Parts)
                    {
                        if (part.Type != null)
                        {
                            if (!this._setTypes[Child.Type].Sets[set.Id].Parts.ContainsKey(part.Id + "-" + part.Type))
                                this._setTypes[Child.Type].Sets[set.Id].Parts.Add(part.Id + "-" + part.Type,
                              new Part(part.Id, SetTypeUtility.GetSetType(Child.Type), part.Colorable, part.Index, part.Colorindex));
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

                string rebuildFigure = string.Empty;

                #region Check clothing, colors & Club
                string[] figureParts = figure.Split('.');
                foreach (string part in figureParts.ToList())
                {
                    if (!part.Contains('-'))
                    {
                        continue;
                    }

                    string type = part.Split('-')[0];

                    if (this._setTypes.TryGetValue(type, out FigureSet figureSet))
                    {
                        string[] splitpart = part.Split('-');
                        if (splitpart.Length < 2)
                        {
                            continue;
                        }

                        int partId = Convert.ToInt32(splitpart[1]);
                        int colorId = 0;
                        int secondColorId = 0;

                        if (figureSet.Sets.TryGetValue(partId, out Set set))
                        {
                            #region Gender Check
                            if (set.Gender != gender && set.Gender != "U")
                            {
                                if (figureSet.Sets.Count(x => x.Value.Gender == gender || x.Value.Gender == "U") > 0)
                                {
                                    partId = figureSet.Sets.FirstOrDefault(x => x.Value.Gender == gender || x.Value.Gender == "U").Value.Id;

                                    //Fetch the new set.
                                    figureSet.Sets.TryGetValue(partId, out set);

                                    colorId = this.GetRandomColor(figureSet.PalletId);
                                }
                                else
                                {
                                    //No replacable?
                                }
                            }
                            #endregion

                            #region Colors
                            if (set.Colorable)
                            {
                                //Couldn't think of a better way to split the colors, if I looped the parts I still have to remove Type-PartId, then loop color 1 & color 2. Meh

                                int splitterCounter = part.Count(x => x == '-');
                                if (splitterCounter == 2 || splitterCounter == 3)
                                {
                                    #region First Color
                                    if (!string.IsNullOrEmpty(part.Split('-')[2]))
                                    {
                                        if (int.TryParse(part.Split('-')[2], out colorId))
                                        {
                                            colorId = Convert.ToInt32(part.Split('-')[2]);

                                            Palette palette = this.GetPalette(colorId);
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
                                    #endregion
                                }

                                if (splitterCounter == 3)
                                {
                                    #region Second Color
                                    if (!string.IsNullOrEmpty(part.Split('-')[3]))
                                    {
                                        if (int.TryParse(part.Split('-')[3], out secondColorId))
                                        {
                                            secondColorId = Convert.ToInt32(part.Split('-')[3]);

                                            Palette palette = this.GetPalette(secondColorId);
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
                                    #endregion
                                }
                            }
                            else
                            {
                                string[] ignore = new string[] { "ca", "wa" };

                                if (ignore.Contains(type))
                                {
                                    int splitterCounter = part.Count(x => x == '-');
                                    if (splitterCounter > 1)
                                    {
                                        if (!string.IsNullOrEmpty(part.Split('-')[2]))
                                        {
                                            colorId = Convert.ToInt32(part.Split('-')[2]);
                                        }
                                    }
                                }
                            }
                            #endregion

                            if (set.ClubLevel > 0 && !hasClub)
                            {
                                partId = figureSet.Sets.FirstOrDefault(x => x.Value.Gender == gender || x.Value.Gender == "U" && x.Value.ClubLevel == 0).Value.Id;

                                figureSet.Sets.TryGetValue(partId, out set);

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
                #endregion

                #region Check Required Clothing
                foreach (string requirement in this._requirements)
                {
                    if (!rebuildFigure.Contains(requirement))
                    {
                        if (requirement == "ch" && gender == "M")
                        {
                            continue;
                        }

                        if (this._setTypes.TryGetValue(requirement, out FigureSet figureSet))
                        {
                            Set set = figureSet.Sets.FirstOrDefault(x => x.Value.Gender == gender || x.Value.Gender == "U").Value;
                            if (set != null)
                            {
                                int partId = figureSet.Sets.FirstOrDefault(x => x.Value.Gender == gender || x.Value.Gender == "U").Value.Id;
                                int colorId = this.GetRandomColor(figureSet.PalletId);

                                rebuildFigure = rebuildFigure + requirement + "-" + partId + "-" + colorId + ".";
                            }
                        }
                    }
                }
                #endregion

                return rebuildFigure.Remove(rebuildFigure.Length - 1);
            }
            catch { }
            return "hd-180-1.lg-270-1408";
        }

        public Palette GetPalette(int colorId)
        {
            return this._palettes.FirstOrDefault(x => x.Value.Colors.ContainsKey(colorId)).Value;
        }

        public bool TryGetPalette(int palletId, out Palette palette)
        {
            return this._palettes.TryGetValue(palletId, out palette);
        }

        public int GetRandomColor(int palletId)
        {
            return this._palettes[palletId].Colors.FirstOrDefault().Value.Id;
        }
    }
}
