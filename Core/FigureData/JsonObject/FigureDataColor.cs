using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WibboEmulator.Core.FigureData.JsonObject
{
    public class FigureDataColor
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public int Club { get; set; }
        public bool Selectable { get; set; }
        public string HexCode { get; set; }
    }
}
