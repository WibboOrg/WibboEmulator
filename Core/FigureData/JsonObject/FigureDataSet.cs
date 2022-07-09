using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wibbo.Core.FigureData.JsonObject
{
    public class FigureDataSet
    {
        public int Id { get; set; }
        public string Gender { get; set; }
        public int Club { get; set; }
        public bool Colorable { get; set; }
        public bool Selectable { get; set; }
        public bool Preselectable { get; set; }
        public bool Sellable { get; set; }
        public List<FigureDataPart> Parts { get; set; }
        public List<FigureDataHiddenLayer> HiddenLayers { get; set; }
    }
}
