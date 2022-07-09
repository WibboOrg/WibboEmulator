using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wibbo.Core.FigureData.JsonObject
{
    public class FigureDataRoot
    {
        public List<FigureDataPalette> Palettes { get; set; }
        public List<FigureDataSetType> SetTypes { get; set; }
    }
}
